using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    ObjectPooler objectPooler;
    
    public float timeForSpawn = 0;
    public float timeSpawning = 0.05f;

    private void Start()
    {
        transform.position = new Vector2(4.0f, 7.0f);
        objectPooler = ObjectPooler.Instance;
    }

    

    void Update()
    {
        if (timeForSpawn > 0)
        {
            timeForSpawn -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if(timeForSpawn <= 0){

            if (timeSpawning > 0)
            {
                timeSpawning -= Time.deltaTime;
            }           
            
            if(timeSpawning>0){

                objectPooler.SpawnFromPool("Box", transform.position, Quaternion.identity);

            }else{
                timeForSpawn = 10;
                timeSpawning= 0.05f;
            }
        }
        
    }
}
