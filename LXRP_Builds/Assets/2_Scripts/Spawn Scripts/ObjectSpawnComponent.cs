using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;
using Random = UnityEngine.Random;

public class ObjectSpawnComponent : MonoBehaviour
{
    public IPoolInfo poolInfo;

    public void InitialzePool(IPoolInfo poolClassObject, int seed, GameObject[] prefabArray)
    {
        poolInfo = poolClassObject;

        Random.InitState(seed);

        //Debug.Log("Intitializing " + poolInfo.WhatToSpawn.ToString() + " started");

        // Initialize Object Pool
        for (int i = 0; i < poolInfo.PoolAmt; i++)
        {
            GameObject spawnObject = Instantiate(prefabArray[Random.Range(0, prefabArray.Length)]);
            spawnObject.SetActive(false);
            spawnObject.transform.parent = this.gameObject.transform;

            //-------------------------------------------------------------------------
            // Choose a random path in the scene
            PathCreator path = poolInfo.PathsArray[Random.Range(0, poolInfo.PathsArray.Length)];
            spawnObject.GetComponent<PathFollower>().pathCreator = path;

            //---------------------------------------------------------------------------

            poolClassObject.PoolDictionery.Add(spawnObject, path);

            // Make it child of the path gameobject
            spawnObject.transform.parent = path.gameObject.transform;
        }
        //Clear prefabArray
        System.Array.Clear(prefabArray, 0, prefabArray.Length);
        prefabArray = null;
    }

    public void StartSpawn()
    {
        StartCoroutine(SpawnCountdown());
    }


    IEnumerator SpawnCountdown()
    {
        yield return new WaitForSeconds(1.0f);
        //Debug.Log(poolInfo.WhatToSpawn.ToString() + " Spawnner started");

        foreach(GameObject spawnObject in poolInfo.PoolDictionery.Keys)
        {
            if (poolInfo.CanSpawn)
            {
                //Debug.Log("Spawnning " + spawnObject.name);

                //yield return new WaitForSeconds(Random.Range(2.0f , poolInfo.MaxSpawnDelay));
                yield return new WaitForSeconds(0.5f);

                // Skip if already active
                if (spawnObject.activeSelf)
                    continue;

                spawnObject.SetActive(true);
            }
        }
    }
    

}

