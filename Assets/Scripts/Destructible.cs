using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, Shootable
{
    public void Shot()
    {
        Destroy(gameObject);
    }
}
