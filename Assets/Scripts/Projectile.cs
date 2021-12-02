using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public Transform target;
    public GameObject explosion;
    public float speed = 5f;
    public float decel = 12f;
    public float normalDecel = 3f;
    public float maxSpeed = 20f;
    public float rotateSpeed = 90f;

    void Start()
    {
        if(target == null) target = GameObject.Find("Player").transform;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.TryGetComponent(out PlayerController p))
        {
            p.Die();
        }
        else
        {
            // we exploded. oops
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotateSpeed);
        rb.AddForce(-Vector3.ProjectOnPlane(rb.velocity, transform.forward) * normalDecel, ForceMode.Force);
        if(Vector3.Project(rb.velocity, transform.forward).magnitude > maxSpeed)
        {
            rb.AddRelativeForce(-Vector3.forward * decel, ForceMode.Force);
        }
        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
    }
}
