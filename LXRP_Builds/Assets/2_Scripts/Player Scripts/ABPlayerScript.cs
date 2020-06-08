// Super class to handle function for player actions and collisions
using System.Collections;
using UnityEngine;
using PathCreation;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(PlayerAnimController))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathCreation.PathFollower))]
[RequireComponent(typeof(NavMeshAgent))]

public abstract class ABPlayerScript : MonoBehaviour, IClickable
{
    public delegate void OnGameEvent(int i);
    public static event OnGameEvent onGameEvent;

    private Outline outlineComponent;
    private PlayerAnimController animController;
    private PathFollower pathFollower;
    private NavMeshAgent navMeshAgent;

    [SerializeField] GameObject pointerComponent = null;
    [SerializeField] SO_PlayerInfo playerInfo = null;
    private bool isDecreaseScoreRunning = false;
    private bool isOnCrossing = false;
    private bool isOnRoad;
    private bool rulebooksSpawned = false;

    public SO_PlayerInfo PlayerInfo { get => playerInfo; }

    private void Awake()
    {
        outlineComponent = GetComponent<Outline>();
        animController = GetComponent<PlayerAnimController>();
        pathFollower = GetComponent<PathFollower>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        
    }

    private void Start()
    {
        if (!CheckRefs())
            return;

        playerInfo.attachedObject = gameObject;
        SwitchComponents(false);
    }

    private void OnEnable()
    {
        pathFollower.enabled = true;
    }

    // Process events when player begins collision with other objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Vehicle") // Vechicle hit
        {
            //Debug.Log("Player got hit by a vehicle");
        }
        else if (other.gameObject.tag.Substring(0, 4) == "Book") // Interact with rulebook
        {
            RulesScript ruleBook = other.GetComponent<RulesScript>();
            ruleBook.CollideWithPlayer();
        }
        else if (other.gameObject.tag == "PedestrianBalcombe") // Level crosssing on Balcombe Road
        {
            MainManager.Instance.SetVehicleSpeed(0.0f);
            isOnCrossing = true;
        }
        else if (other.gameObject.tag == "ComoCrossing") // Level crossing on Como Parade
        {
            MainManager.Instance.SetVehicleSpeed(0.0f);
            isOnCrossing = true;
        }
        else if (other.gameObject.tag == "Road") // Player runs on road
        {
            if (MainManager.Instance.GetState() == EGameState.QUEST_START) // Only activate when level has started
            {
                isOnRoad = true;

                if (!isOnCrossing) // Check to see if player is on level crossing
                    StartCoroutine(DecreaseScore());
            }
        }
        //Hit by Train
        else if(other.gameObject.tag == "Train")   
        {
            MainManager.Instance.UpdateScore(EScoreEvent.HIT_BY_TRAIN);
        }
        else if (other.gameObject.tag == "Station") // "Arrive" at station at end of Level 1
        {
            if (MainManager.Instance.CurrSelectedPlayer.PlayerInfo.characterMission == EMissionType.GET_TO_STATION)
            {

                Debug.Log("Player is :"+ MainManager.Instance.CurrSelectedPlayer.PlayerInfo.characterName);
                Debug.Log("Entered station quest completion");
                Outline stationOutline = other.GetComponent<Outline>();
                stationOutline.OutlineWidth = 0.0f;
                SphereCollider collider = other.GetComponent<SphereCollider>();
                collider.gameObject.SetActive(false);
                MainManager.Instance.UpdateScore(EScoreEvent.AT_STATION);
                MainManager.Instance.SetState(EGameState.QUEST_COMPLETE);
            }
        }
        else if (other.gameObject.tag == "InfoHub") // "Arrive" at InforHub at end of Level 3
        {
            if (UIManager.Instance.AllDonutsCollected)
            {
                MainManager.Instance.SetState(EGameState.PLAYER_COMPLETE);
            }
        }

        else if (other.gameObject.tag.Substring(0, 14) == "Level3Question") // "Collecting" a donut on Level 3
        {
            // Only enforce collision when playing Level 3 mission 
            if (MainManager.Instance.CurrSelectedPlayer.PlayerInfo.characterMission == EMissionType.ANSWER_QUESTIONS) 
            {
                int questionNo = Int32.Parse(other.gameObject.tag.Substring(14, 1));
                questionNo--;
                Debug.Log("Question Number: " + questionNo);
                UIManager.Instance.ShowQuestionUI(questionNo);
            }
        }
    }

    // Process events when player continues started collisions with other objects
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Road") // Player staying on road
        {
            // Enforce only when a level has started
            if (MainManager.Instance.GetState() == EGameState.QUEST_START)
            {
                isOnRoad = true;

                if (!isOnCrossing) // Player still on road but not using a level crossing
                    StartCoroutine(DecreaseScore());
            }
        }
    }

    // Coroutine to reduce score when player is on the road
    IEnumerator DecreaseScore()
    {
        if (!isDecreaseScoreRunning && isOnRoad)
        {
            isDecreaseScoreRunning = true;

            while (isOnRoad)
            {
                yield return new WaitForSeconds(1.5f);

                MainManager.Instance.UpdateScore(EScoreEvent.ON_ROAD);
                isDecreaseScoreRunning = false;
            }
        }
    }

    // Process events when player ends collision with other objects
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Road")
        {
            // Enforce only when level has started
            if (MainManager.Instance.GetState() == EGameState.QUEST_START)
            {
                isOnRoad = false; // Flag that player is no longer standing on the road
            }
        }
        else if (other.gameObject.tag == "PedestrianBalcombe") // Player stepped off Balcombe Road Level Crossing
        {
            isOnCrossing = false;
            MainManager.Instance.SetVehicleSpeed(1.3f);
        }
        else if (other.gameObject.tag == "ComoCrossing") // Player stepped off Como Parade crossing
        {
            isOnCrossing = false;
            MainManager.Instance.SetVehicleSpeed(1.3f);
        }
    }

    // Check components on player game object are initialised
    private bool CheckRefs()
    {
        bool check = true;
        
        if (outlineComponent == null)
        {
            Debug.LogError("Outline : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (animController == null)
        {
            Debug.LogError(" PlayerAnimController : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (pointerComponent == null)
        {
            Debug.LogError("Pointer : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (pathFollower == null)
        {
            Debug.LogError("PathFollower : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (PlayerInfo == null)
        {
            Debug.LogError("Character UI Info : Ref Missing - on " + transform.name);
            check = false;
        }
        return check;
    }

    // Toggle to enable/disbale path, pointer and outline components
    public void SwitchComponents(bool condition)
    {
        outlineComponent.enabled = !condition;
        animController.enabled = condition;
        pointerComponent.SetActive(condition);

        pathFollower.enabled = !condition;
    }

    // Event handler for mouse click on player 
    public void OnClick()
    {
       if (MainManager.Instance.CurrSelectedPlayer != this)
            return;

        SwitchComponents(true); // Enable pointer & path follower and disable outline components

        EnactSpecificMissionActions();
    }

    // Spawn level specifics objects once player has been "selected"
    private void EnactSpecificMissionActions()
    { 
        switch (MainManager.Instance.CurrSelectedPlayer.PlayerInfo.characterMission)
        {
            case EMissionType.GET_TO_STATION: // Level 1
                GameObject station = GameObject.FindGameObjectWithTag("Station");
                Outline outline = station.GetComponent<Outline>();
                outline.OutlineWidth = 7.0f;
                break;
            case EMissionType.FIND_CORRECT_RULES: // Level 2
                if (!rulebooksSpawned)
                {
                    SpawnManager.Instance.StartSpawn(ESpawnSelection.RULES);
                    rulebooksSpawned = true;
                }
                break;
            case EMissionType.ANSWER_QUESTIONS: // Level 3
                UIManager.Instance.ShowDonuts(true);
                break;
            default:
                break;
        }
    }

    // Utility function to message manager
    public void SendMessageToManager()
    {
        throw new System.NotImplementedException();
    }

    // Set the navmesh agent component to a new position 
    public void MoveToDestination(Vector3 pos)
    {
        if (!navMeshAgent.isOnNavMesh)          //check if NavMeshAgent is on Navmesh/ground 
            return;
        navMeshAgent.SetDestination(pos);
    }
}
