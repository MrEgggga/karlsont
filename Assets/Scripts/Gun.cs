using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public Rigidbody rb;
    public ParticleSystem ps;
    public float cooldown;
    public float impulse;
    public int ammo;

    public LayerMask layerMask;

    public int pellets;
    public float spread;

    public GameObject bullet;

    public Image[] ammoIndicators;
    
    private int ammoLeft;
    private float shootTimer;

    public LayerMask grappleLayerMask;
    public float grappleAimAssist;
    public float grappleForce;
    public float grappleNormalResistance;
    private bool grappling;
    private Vector3 grapplePoint;
    private Collider grapplingTo;
    public LineRenderer rope;

    public PlayerInput input;
    private InputAction fire;
    private InputAction grapple;
    private InputAction slide;

    public AudioSource shotSound;

    void Start()
    {
        fire = input.actions.FindAction("Fire");
        grapple = input.actions.FindAction("Grapple");
        slide = input.actions.FindAction("Slide");

        ammoLeft = ammo;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < ammoIndicators.Length; ++i)
        {
            if(ammoLeft > i)
            {
                ammoIndicators[i].color = new Color(1, 1, 1);
            }
            else
            {
                ammoIndicators[i].color = new Color(1, 1, 1, 0.3f);
            }
        }

        if(shootTimer <= 0f)
        {
            if(fire.ReadValue<float>() > 0.5f && ammoLeft > 0)
            {
                // shoot
                // if(shotSound != null) shotSound.Play();

                for(int i = 0; i < pellets; ++i)
                {
                    float angle = Random.Range(0, Mathf.PI * 2);
                    float magnitude = Random.Range(0f, 1f);
                    Vector2 shotPos = new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
                    Vector3 shotDir = new Vector3(shotPos.x, shotPos.y, 1/Mathf.Tan(Mathf.Deg2Rad * spread / 2));

                    Quaternion rot = Quaternion.LookRotation(shotDir) * transform.rotation;
                    GameObject newBullet = Instantiate(bullet, transform.position, rot);

                    if(Physics.Raycast(transform.position, 
                        transform.rotation * shotDir, 
                        out RaycastHit hit, layerMask))
                    {
                        Debug.Log(hit.collider.name);
                        if(hit.collider.TryGetComponent(out Shootable shootable))
                        {
                            shootable.Shot();
                        }
                    }
                }

                Vector3 velocityAlongShootDir = Vector3.Project(rb.velocity, transform.forward);
                float maxImpulse = slide.ReadValue<float>() > 0.9f ? 
                    impulse :
                    Mathf.Max(impulse, ((-transform.forward * impulse) - velocityAlongShootDir).magnitude);

                rb.AddForce(-transform.forward * maxImpulse, ForceMode.Impulse);
                ps.Play();

                shootTimer = cooldown;
                ammoLeft -= 1;
            }
        }
        else
        {
            shootTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if(grapple.ReadValue<float>() > 0.9f)
        {
            Debug.Log("Grappling");
            if(!grappling)
            {
                // Increase radius of sphere cast, up to grappleAimAssist
                // thus, if we are pointing directly at a target but
                // there is a closer target within the grappleAimAssist
                // radius, we will grapple to the far-away object; however
                // if we are not we will have aim assist
                // This allows for larger maximum aim assist without things
                // getting in the way
                for(float r = 0f; r < grappleAimAssist; r += 1f)
                {
                    if(Physics.SphereCast(transform.position, r, transform.forward, out RaycastHit hit, grappleLayerMask))
                    {
                        if(hit.collider.gameObject.GetComponent<Grapplable>())
                        {
                            grappling = true;
                            grapplingTo = hit.collider;
                            grapplePoint = hit.point;
                            break;
                        }
                    }
                }
            }
            if(grappling)
            {
                if(grapplingTo == null)
                {
                    grappling = false;
                    return;
                }

                // Vector we're grappling in
                Vector3 dir = (grapplePoint - transform.position).normalized;
                // Resist forces orthogonal to the grapple direction
                rb.AddForce(-Vector3.ProjectOnPlane(rb.velocity, dir) * grappleNormalResistance, ForceMode.Force);
                // Add a force towards the grapple point
                rb.AddForce(dir * grappleForce, ForceMode.Force);

                // Render the rope
                rope.gameObject.SetActive(true);
                rope.SetPosition(0, transform.position + transform.forward * 0.35f);
                rope.SetPosition(1, grapplePoint);
            }
        }
        else
        {
            rope.gameObject.SetActive(false);
            grappling = false;
        }
    }

    public void Restock()
    {
        ammoLeft = ammo;
    }
}
