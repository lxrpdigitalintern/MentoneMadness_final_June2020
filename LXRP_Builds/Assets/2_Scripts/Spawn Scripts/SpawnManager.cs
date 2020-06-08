// Manager class to store and handle the game object spawns 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class SpawnManager : MonoBehaviour
{
    // Singleton Members
    private static SpawnManager _instance;
    public static SpawnManager Instance { get { return _instance; } }

    // Class members
    [SerializeField] private ObjectSpawnComponent pedestrianComponent = null;
    [SerializeField] int pedestrianPoolAmt = 25;
    [SerializeField] float pedestrianMaxSpawnDelay = 5.0f;
    [SerializeField] PathCreator[] pedestrianPaths = null;

    private GameObject[] pedestriansPrefabs;
    private IPoolInfo pedestrianPoolInfo;

    [Space]
    [SerializeField] private ObjectSpawnComponent vehicleComponent = null;
    [SerializeField] int vehiclePoolAmt = 25;
    [SerializeField] float vehicleMaxSpawnDelay = 5.0f;
    [SerializeField] PathCreator[] vehiclePaths = null;

    private GameObject[] vehiclePrefabs;
    private IPoolInfo vehiclePoolInfo;

    [Space]
    [SerializeField] PlayerObjectsSpawnComponent playerObjectsComponent = null;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        // Initialize Random Seed
        int seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);

        PreparePools(seed);
    }

    // Return scriptable object data for current Question UI question
    public SO_QuestionInfo GetQuestion(int inIndex)
    {
        return playerObjectsComponent.GetQuestion(inIndex);
    }

    // CAN BE REFACTORED
    // Initialise all object spawn pools
    public void PreparePools(int seed)
    {
        // Prepare the poolInfos
        pedestrianPoolInfo = new IPoolInfo(ESpawnSelection.PEDESTRIANS, pedestrianPoolAmt, pedestrianMaxSpawnDelay,
            pedestrianPaths, true);

        // Get all prefabs from the list 
        pedestriansPrefabs = Resources.LoadAll<GameObject>("PEDESTRIANS");
        pedestrianComponent.InitialzePool(pedestrianPoolInfo, seed, pedestriansPrefabs);

        // Same for the vehicles as well
        vehiclePoolInfo = new IPoolInfo(ESpawnSelection.VEHICLES, vehiclePoolAmt, vehicleMaxSpawnDelay,
            vehiclePaths, true);
        vehiclePrefabs = Resources.LoadAll<GameObject>("VEHICLES");
        vehicleComponent.InitialzePool(vehiclePoolInfo, seed, vehiclePrefabs);
    }

    // Spawn specified game object type
    public void StartSpawn(ESpawnSelection whatToSpawn)
    {
        switch(whatToSpawn)
        {
            case ESpawnSelection.PEDESTRIANS:
                pedestrianComponent.StartSpawn();
                break;

            case ESpawnSelection.VEHICLES:
                vehicleComponent.StartSpawn();
                break;

            case ESpawnSelection.RULES:
                playerObjectsComponent.SpawnRules();
                break;

            case ESpawnSelection.PLAYERS:
                playerObjectsComponent.SpawnPlayer();
                break;
        }
    }

    // Select random path to assign to newly spawned vehicle
    public PathCreator GetRandomVehiclePath()
    {
        return vehiclePaths[Random.Range(0, vehiclePaths.Length)];
    }

    // Select random path to assign to newly spawned pedestrian
    public PathCreator GetRandomPedestrianPath()
    {
        return pedestrianPaths[Random.Range(0, pedestrianPaths.Length)];
    }

    // Obtain current number of specified objects
    public int getNoOfSpawns(ESpawnSelection whichObject)
    {
        switch(whichObject){

            case ESpawnSelection.RULES:
                return playerObjectsComponent.AmtOfRules;

            case ESpawnSelection.PLAYERS:
                return playerObjectsComponent.AmtOfPlayers;
        }
        return 0;
    }
}

