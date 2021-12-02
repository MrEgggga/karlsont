using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTowardsTarget : MonoBehaviour
{
    public Transform target;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        if(target == null) target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        parent.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        transform.localRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0f, 0f);
    }
}
