using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impulse;
    public float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * impulse, ForceMode.Impulse);
        Invoke("DestroySelf", lifeTime);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
