using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float impulseThreshold;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > impulseThreshold)
        {
            if(collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(-collision.impulse + (collision.impulse.normalized * impulseThreshold), ForceMode.Impulse);
            }
            Destroy(gameObject);
        }
    }
}
