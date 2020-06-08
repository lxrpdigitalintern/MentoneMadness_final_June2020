using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// Interface Class containting all the necessary information for object spawnner.
public class IPoolInfo
{
    private readonly ESpawnSelection whatToSpawn;

    readonly private int poolAmt;
    private Dictionary<GameObject, PathCreator> poolDictionery;

    private readonly PathCreator[] pathsArray;
    readonly private float maxSpawnDelay;

    private bool canSpawn;

    public IPoolInfo(ESpawnSelection objectsToSpawn, int poolAmt, float maxSpawnDelay, PathCreator[] pathsArray, bool canSpawn = true)
    {
        this.whatToSpawn = objectsToSpawn;

        this.poolAmt = poolAmt;
        this.poolDictionery = new Dictionary<GameObject, PathCreator>();

        this.pathsArray = pathsArray;

        this.maxSpawnDelay = maxSpawnDelay;
        this.canSpawn = canSpawn;
    }

    public ESpawnSelection WhatToSpawn => whatToSpawn;

    public int PoolAmt => poolAmt;
    public Dictionary<GameObject, PathCreator> PoolDictionery { get => poolDictionery; set => poolDictionery = value; }

    public PathCreator[] PathsArray => pathsArray;
    public float MaxSpawnDelay => maxSpawnDelay;

    public bool CanSpawn { get => canSpawn; set => canSpawn = value; }
}