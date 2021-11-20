using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float impulseThreshold;
    public float upwardImpulse;

    public GameObject explosion;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > impulseThreshold)
        {
            if(collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(-collision.impulse + (collision.impulse.normalized * impulseThreshold), ForceMode.Impulse);
                rb.AddForce(Vector3.up * (upwardImpulse + (collision.impulse.y > 0 ? collision.impulse.y : 0)), ForceMode.Impulse);
            }
            if(collision.gameObject.TryGetComponent(out PlayerController p))
            {
                p.gun.Restock();
            }
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
