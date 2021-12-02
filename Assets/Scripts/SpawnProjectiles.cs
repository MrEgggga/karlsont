using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{
    public Transform target;
    public float spawnRate;
    public GameObject projectile;
    public float offset;

    // Start is called before the first frame update
    void Start()
    {
        if(target == null) target = GameObject.Find("Player").transform;
        InvokeRepeating("SpawnProjectile", spawnRate, spawnRate);
    }

    void SpawnProjectile()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            if(hit.collider.transform == target)
            {
                Instantiate(projectile, transform.position + transform.forward * offset, transform.rotation);
            }
        }
    }
}
