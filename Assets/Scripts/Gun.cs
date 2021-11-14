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

    public PlayerInput input;
    private InputAction fire;

    void Start()
    {
        fire = input.actions.FindAction("Fire");
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

                rb.AddForce(transform.forward * impulse * -1, ForceMode.Impulse);
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

    public void Restock()
    {
        ammoLeft = ammo;
    }
}
