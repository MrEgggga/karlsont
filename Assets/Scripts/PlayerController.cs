using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    enum State
    {
        Air,
        Ground,
        Slide,
        Wallrun
    }

    public float minimumY = -10f;

    public float acceleration;
    public float deceleration;
    public float maxSpeed;
    public float jumpForce;
    public float wallrunNormalForce;
    public float wallrunCameraTilt;

    public Collider standingCollider;
    public Collider slidingCollider;
    public Transform mouseLook;
    public Gun gun;

    private Rigidbody rb;
    [SerializeField] private State state;

    private Vector3 wallrunNormal;

    private PlayerInput input;
    private InputAction move;
    private InputAction slide;
    private InputAction jump;
    private InputAction restart;

    private Dictionary<Collider, ContactPoint> contacts;

    void OnCollisionEnter(Collision collision)
    {
        // Add key to dict
        if(!contacts.ContainsKey(collision.collider))
        {
            contacts.Add(collision.collider, collision.GetContact(0));
        }
    }

    void OnCollisionStay(Collision collision)
    {
        contacts[collision.collider] = collision.GetContact(0);
    }

    void OnCollisionExit(Collision collision)
    {
        contacts.Remove(collision.collider);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get all the inputs
        input = GetComponent<PlayerInput>();
        move = input.actions.FindActionMap("Player").FindAction("Move");
        slide = input.actions.FindActionMap("Player").FindAction("Slide");
        jump = input.actions.FindActionMap("Player").FindAction("Jump");
        restart = input.actions.FindActionMap("Player").FindAction("Restart");

        rb = GetComponent<Rigidbody>();

        contacts = new Dictionary<Collider, ContactPoint>();
    }

    private void FixedUpdate()
    {
        bool grounded = false;
        List<Collider> toRemove = new List<Collider>();
        // Update state
        // Look at each contact and check its normal to
        // see which state we should be in
        foreach(KeyValuePair<Collider, ContactPoint> contact in contacts)
        {
            // Outdated contact
            if(contact.Key == null)
            {
                // Add to to-remove list; since we're iterating
                // through the list we can't remove it now
                toRemove.Add(contact.Key);
            }
            // Normal Y is mostly positive so we're on
            // relatively flat ground
            // TODO: fix these magic constants
            if(contact.Value.normal.y > 0.7f)
            {
                // On ground
                // Sliding?
                if(slide.ReadValue<float>() >= 0.9f)
                {
                    state = State.Slide;
                }
                else
                {
                    state = State.Ground;
                }
                grounded = true;
                gun.Restock();
                break;
            }
            // Normal is very small, we're touching a wall. do wallrun things
            else if(Mathf.Abs(contact.Value.normal.y) < 0.2f)
            {
                state = State.Wallrun;
                wallrunNormal = contact.Value.normal;
                grounded = true;
            }
        }

        // Remove outdated contacts
        foreach(Collider col in toRemove)
        {
            contacts.Remove(col);
        }

        // If we don't have any contacts that fit, use air state
        if(!grounded) state = State.Air;

        // * Do things based on state:
        // Aerial:
        // - Half acceleration
        // - No deceleration
        // Sliding:
        // - No acceleration
        // - No deceleration
        // - Small hitbox (sphere)
        // Grounded:
        // - Full acceleration
        // - Full deceleration
        // Wallrun:
        // - No acceleration
        // - No deceleration

        if(state == State.Slide)
        {
            standingCollider.enabled = false;
            slidingCollider.enabled = true;
            mouseLook.localPosition = Vector3.down * 0.3f;
        }
        else
        {
            standingCollider.enabled = true;
            slidingCollider.enabled = false;
            mouseLook.localPosition = Vector3.up * 0.3f;
        }

        // Wallrun camera tilt
        if(state == State.Wallrun)
        {
            Vector3 relativeWallrunNormal = Quaternion.Inverse(transform.rotation) * wallrunNormal;
            if(relativeWallrunNormal.x > 0.8f)
            {
                mouseLook.GetComponent<MouseLook>().tilt = Quaternion.Euler(0, 0, wallrunCameraTilt);
            }
            else if(relativeWallrunNormal.x < -0.8f)
            {
                mouseLook.GetComponent<MouseLook>().tilt = Quaternion.Euler(0, 0, -wallrunCameraTilt);
            }
            else
            {
                mouseLook.GetComponent<MouseLook>().tilt = Quaternion.identity;
            }
        }
        else
        {
            mouseLook.GetComponent<MouseLook>().tilt = Quaternion.identity;
        }

        // Acceleration multiplier
        float accelerationMultiplier = state == State.Slide || state == State.Wallrun ? 0f : (state == State.Air ? 0.5f : 1f);

        Vector2 movement = move.ReadValue<Vector2>();
        rb.AddRelativeForce((Vector3.forward * movement.y + Vector3.right * movement.x) * acceleration * accelerationMultiplier, ForceMode.Force);

        // Deceleration if we're not holding a direction or
        // we're going to fast; runs only in ground state
        // TODO: duplicated condition in if
        if(state == State.Ground && movement.magnitude <= 0.2f)
        {
            rb.AddForce(-rb.velocity * deceleration, ForceMode.Force);
        }
        else if(state == State.Ground && rb.velocity.magnitude >= maxSpeed)
        {
            rb.AddForce(-rb.velocity * deceleration * 0.8f, ForceMode.Force);
        }

        // Very unDRY jump code
        // TODO: this code is a mess
        if(jump.ReadValue<float>() >= 0.9f)
        {
            if((state == State.Ground || state == State.Slide))
            {
                if(rb.velocity.y < 0f) {
                    rb.AddForce(Vector3.up * -rb.velocity.y, ForceMode.Impulse);
                }
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if(state == State.Wallrun)
            {
                if(rb.velocity.y < 0f) {
                    rb.AddForce(Vector3.up * -rb.velocity.y, ForceMode.Impulse);
                }
                rb.AddForce(Vector3.up * jumpForce + 
                    wallrunNormal * wallrunNormalForce, ForceMode.Impulse);
            }
        }

        // Death plane in case I forget to add it to the map
        if(transform.position.y < minimumY)
        {
            Die();
        }

        // Restart if we want to
        if(restart.ReadValue<float>() > 0.5f)
        {
            Die();
        }
    }

    // TODO: maybe wait before reloading scene, add death anim, etc.
    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
