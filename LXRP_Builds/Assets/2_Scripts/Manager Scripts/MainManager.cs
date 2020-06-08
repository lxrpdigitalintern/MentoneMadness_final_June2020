using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.SceneManagement;

// Singleton class
public class MainManager : MonoBehaviour
{
    // Singleton Members
    private static MainManager _instance;
    public static MainManager Instance { get { return _instance; } }
    
    // Main selected character
     ABPlayerScript currSelectedPlayer;
    public ABPlayerScript CurrSelectedPlayer { get => currSelectedPlayer; }

    private EGameState managerState = EGameState.BLANK;
    private List<SO_RuleInfo> selectedRules = new List<SO_RuleInfo>();

    [SerializeField] WorldPlacementScript placementScript = null;
    [SerializeField] float LevelStartDelay = 6.0f;
    [SerializeField] float LevelEndDelay = 4.0f;

    public int currentLevel = 0;
    
    // constants
    //int levelOnePartTwoIndex = 1;
    const int NUM_RULEBOOKS = 5;
    const int FINAL_LEVEL = 3; 

    private int score = 0;

    #region Private Methods

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

    private void Start()                 
    {
        SetState(EGameState.BEGIN);
    }


    // Function to handle state changes
    private void HandleStateChangedEvent(EGameState state)
    {
        switch (state)
        {
            // Set game world 
            case EGameState.BEGIN:
                InitializeGame();
                break;
            // Enable Game & spawn game world objects
            case EGameState.PLACED:
                InitLevel();
                break;
            // Set up next level player character
            case EGameState.PLAYER_START:
                SetupNewPlayerCharacter();
                break;
            // Begin the next level
            case EGameState.QUEST_START:
                Debug.Log("Completed level is :"+ currentLevel);
                currentLevel++;
                Debug.Log("Next level is :"+ currentLevel);
                StartMission(currSelectedPlayer.PlayerInfo.characterMission);
                break;
            // Perform actions to complete current level
            case EGameState.QUEST_COMPLETE:
                Debug.Log("Quest completed");
                EndMission();
                break;
            // When the player completes the game
            case EGameState.PLAYER_COMPLETE:
                Destroy(currSelectedPlayer);
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                UIManager.Instance.DisplayGameFinishedMessage();
                Debug.Log("Game completed, player reached the hub");
                //SceneManager.LoadScene("MainMenu");
                break;
            // When the player's score reaches zero -> GAME OVER
            case EGameState.GAME_OVER:
                Destroy(currSelectedPlayer);
                UIManager.Instance.DisplayGameOverMessage();
                break;
            default:
                break;
        }
    }

    // Determine game world placement method
    private void InitializeGame()
    {
        Debug.Log("Platform:" + Application.platform);

#if UNITY_ANDROID
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            placementScript.SetState(EARState.PLACEMENT);
            return;
        }
        placementScript.SetState(EARState.TUTORIAL);
#elif UNITY_IOS
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            placementScript.SetState(EARState.PLACEMENT);
            return;
        }
        placementScript.SetState(EARState.TUTORIAL);
#else
        placementScript.SetState(EARState.PLACEMENT);
#endif
    }
    // Initliase next level - spawn level elements and adjust score
    private void InitLevel()
    {
        UIManager.Instance.InitGameOverMessage();
        SpawnManager.Instance.StartSpawn(ESpawnSelection.PEDESTRIANS);
        SpawnManager.Instance.StartSpawn(ESpawnSelection.VEHICLES);
        SpawnManager.Instance.StartSpawn(ESpawnSelection.PLAYERS);

        UpdateScore(EScoreEvent.GAME_START);
    }

    // Initalize new level elements
    private void StartLevel()
    {
        // Display "Level" label 

            UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_START, currSelectedPlayer.PlayerInfo.characterMission);

        // Delay then display character level instructions
        StartCoroutine(DelayThenStartLevel());
    }

    // Coroutine to delay (2 seconds default) then show level instructions 
    private IEnumerator DelayThenStartLevel()
    {
        yield return new WaitForSeconds(LevelStartDelay);
        UIManager.Instance.HideLevelStatusText();

        // Display Level Instructions UI
        UIManager.Instance.StartIntroSpeech(currSelectedPlayer.PlayerInfo.instructionsSpeechText
           , currSelectedPlayer.PlayerInfo.portraitImage, currSelectedPlayer.PlayerInfo.instructionsIndex
           , currSelectedPlayer.PlayerInfo.introClip);
        SetState(EGameState.QUEST_START);

    }
    
    // Complete end of level actions; message display etc.
    private void EndLevel()
    {
        UIManager.Instance.ShowLevelStatusText();
        UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_COMPLETE, currSelectedPlayer.PlayerInfo.characterMission);
        StartCoroutine(DisplayEndLevelDelay());
    }

    // Coroutine to delay end of level message display and end of level actions
    private IEnumerator DisplayEndLevelDelay()
    {
        yield return new WaitForSeconds(LevelEndDelay);
        Destroy(currSelectedPlayer);
        Destroy(GameObject.FindWithTag("Player"));
        if (currentLevel == FINAL_LEVEL) // Have finished game's last level
            MainManager.Instance.SetState(EGameState.PLAYER_COMPLETE); // Begin end game actions
        else
           SpawnManager.Instance.StartSpawn(ESpawnSelection.PLAYERS); // spawn next level player character
            
    }

    // Set up the player appropriate to level
    private void SetupNewPlayerCharacter()
    {
        if (currSelectedPlayer != null)
        {
            // Enable UI 
            UIManager.Instance.SetPlayerInfo(currSelectedPlayer.PlayerInfo);
            
            StartLevel();
        }
    }

    // Set Camera to specified position vector
    private void PositionCamera()
    {
        const float baseVectorValue = 0.0f;

        // Define position vector and get camera object
        Vector3 pos = new Vector3(baseVectorValue, baseVectorValue, baseVectorValue);
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");

        // Offset values for new camera pos
        const float xoffset = -0.11f;
        const float yoffset = 0.3f;
        const float zoffset = -0.06f;
        const float angle = 90.0f;

        // Create new vector with x,y,z offset values
        Vector3 offset = new Vector3(xoffset, yoffset, zoffset);
        // Add offset vector to player position vector
        pos += offset;
        Quaternion newRot = Quaternion.Euler(0, angle, 0);
        // Set new camera position and rotation
        camera.transform.SetPositionAndRotation(pos, newRot);
    }

    // Enact specifics of a given level
    public void StartMission(EMissionType mission)
    {
        // Position camera 
        PositionCamera();
        
        // Move index further along instructions array for Level 1 Hotdog character
        //if (currSelectedPlayer.PlayerInfo.characterMission == EMissionType.COLLECT_HOTDOGS)
        //    currSelectedPlayer.PlayerInfo.instructionsIndex = levelOnePartTwoIndex;
        
        switch (mission)
        {
            case EMissionType.FIND_CORRECT_RULES:
                UIManager.Instance.Level2Container.gameObject.SetActive(true);
               /* UIManager.Instance.Button2.gameObject.SetActive(true);
                UIManager.Instance.Button3.gameObject.SetActive(true);
                UIManager.Instance.Button4.gameObject.SetActive(true);
                UIManager.Instance.Button5.gameObject.SetActive(true); */
                UIManager.Instance.Level1greenArrows.gameObject.SetActive(false);
               
                break;
            case EMissionType.COLLECT_DONUTS:
                SpawnManager.Instance.StartSpawn(ESpawnSelection.DONUTS);
                break;
            case EMissionType.ANSWER_QUESTIONS:
                SpawnManager.Instance.StartSpawn(ESpawnSelection.QUESTION_CHARACTERS);
                break;
        }
    }

    // Complete actions to end a level
    public void EndMission()
    {
        EndLevel();
    }

#endregion

#region Public Methods

    // Function to change the game state
    public void SetState(EGameState newState)
    {
        if (managerState == newState)
        {
            return;
        }

        managerState = newState;
        HandleStateChangedEvent(managerState);
    }

    // Function to change the game state and set current player character information
    public void SetState(EGameState newState, ABPlayerScript player)
    {
        currSelectedPlayer = player;
        SetState(newState);
    }

    // Accessor to retrieve game state valie
    public EGameState GetState()
    {
        return managerState;
    }

    // Return number of "read" rulebooks
    public int GetNumSelectedRules()
    {
        return selectedRules.Count;
    }

    // Actions when player collides with rulebook object
    public void OnRuleSelect(bool isSelected, SO_RuleInfo info, bool isTrueToggle)
    {
        string tagVal = "";
                
        info.IsSelected = isSelected;
        selectedRules.Add(info);

        if (isSelected)
        {
            tagVal = RulesScript.bookTag;
            Destroy(GameObject.FindGameObjectWithTag(tagVal));
        }
        else
        {
            selectedRules.Remove(info);
            //Debug.Log("_________________________fdxgnfn");
        }
        UIManager.Instance.UpdateRules(selectedRules.Count);
        //UIManager.Instance.ruleAudio.clip = info.question;              //Audio for question
        //UIManager.Instance.ruleAudio.Play();                            //Play the question audio

        if ((isTrueToggle && info.answerIsTrue) || (!isTrueToggle && !info.answerIsTrue))
        {
            UIManager.Instance.ruleAudio.Stop();
            UIManager.Instance.UpdateRulebookText(info.CorrectText);
            UIManager.Instance.AnsAudio.clip = info.rightAns;          //Audio for right answer
            UIManager.Instance.AnsAudio.Play();                        //Play the answer audio
            UpdateScore(EScoreEvent.CORRECT_TRUEORFALSE);
            
        }
        else
        {
            UIManager.Instance.ruleAudio.Stop();
            UIManager.Instance.UpdateRulebookText("Sorry, but that is incorrect.");
            UIManager.Instance.AnsAudio.clip = info.wrongAns;          //Audio for wrong answer
            UIManager.Instance.AnsAudio.Play();                        //Play the answer audio

        }
    }

    // Mutator to alter how fast vehicles move along their path follower component
    public void SetVehicleSpeed(float inSpeed)
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Vehicle");
        foreach (GameObject car in cars)
        {
            car.GetComponent<PathFollower>().speed = inSpeed;
        }
    }
    
    // ALter current score based on game event
    public void UpdateScore(EScoreEvent eScoreEvent)
    {
        switch (eScoreEvent)
        {
            case EScoreEvent.GAME_START: // Init score to 50 at game start
                score = 50;
                UIManager.Instance.UpdateScore(score);
                break;
            case EScoreEvent.ON_ROAD: // Lose points for not using crossing
                if (score > 0)
                    score -= 5;
                UIManager.Instance.UpdateScore(score);
                break;
            case EScoreEvent.AT_STATION: // Got player character to station to end Level 1
                score += 20;
                Debug.Log("Player reached station");
                UIManager.Instance.UpdateScore(score);
                break;
            case EScoreEvent.CORRECT_QUESTION: // Correct A.B or C selected in Level 3 scenario
                score += 10;
                UIManager.Instance.UpdateScore(score);
                break;
            case EScoreEvent.CORRECT_TRUEORFALSE: // True/False statment correct on Level 2 rulebook
                score += 10;
                UIManager.Instance.UpdateScore(score);
                break;
            case EScoreEvent.HIT_BY_TRAIN: //Hit by Train, game Over
                score = 0;
                UIManager.Instance.UpdateScore(score);
                break;
            default:
                break;
        }
        if (score <= 0) // End game if score reaches zero
            SetState(EGameState.GAME_OVER);
    }

    // Accessor to return current score value
    public int GetScore() { return score; }

#endregion
}

