using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int enemiesToSpawn;
    public GameObject enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < enemiesToSpawn; ++i)
        {
            Vector3 pos = new Vector3(
                Random.Range(0f, 100f), 
                Random.Range(0f, 100f), 
                Random.Range(0f, 100f)
            );
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }
}
