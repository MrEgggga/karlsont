using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGate : MonoBehaviour
{
    public Collider col;
    public GameObject[] enemies;
    private bool allDestroyed = false;
    public Vector3 movement;
    public float time;

    // Update is called once per frame
    void Update()
    {
        if(!allDestroyed)
        {
            allDestroyed = true;
            foreach(GameObject g in enemies)
            {
                if(g != null)
                {
                    allDestroyed = false;
                    break;
                }
            }
            if(allDestroyed)
            {
                col.enabled = false;
                StartCoroutine(Open());
            }
        }
    }

    IEnumerator Open()
    {
        int frames = (int) (time * 60);
        for(int i = 0; i < frames; ++i)
        {
            transform.position += movement / frames;
            yield return null;
        }
        yield break;
    }
}
