// Utilty class to handle spawning of player related objects
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectsSpawnComponent : MonoBehaviour
{
    private SO_RuleInfo[] rules = null;
    private SO_QuestionInfo[] questions = null;
    [SerializeField] GameObject rulesPrefab = null;
    [SerializeField] Transform[] spawnLocations = null;

    private int amtOfRules;
    public int AmtOfRules { get => amtOfRules; }

    private int amtOfCorrectRules;
    public int AmtOfCorrectRules { get => amtOfCorrectRules; }

    private int amtOfPlayers = 0;
    public int AmtOfPlayers { get => amtOfPlayers; }
    private int amtOfQuestions;
    public int AmtOfQuestions { get => amtOfQuestions; }

    private GameObject[] playerCharacters;
    [SerializeField]  private int nextPlayer = 0;

    
    private void Awake()
    {
        // Load rules assets into game
        rules = Resources.LoadAll<SO_RuleInfo>("RULES");
        amtOfRules = rules.Length;

        // Load player assets into game
        playerCharacters = Resources.LoadAll<GameObject>("PLAYERS");
        amtOfPlayers = playerCharacters.Length;

        // Load question assets into game
        questions = Resources.LoadAll<SO_QuestionInfo>("QUESTIONS");
        amtOfQuestions = questions.Length;

        // Sort players into correct level order
        Array.Sort(playerCharacters, delegate (GameObject a, GameObject b) { return a.GetComponent<ABPlayerScript>().PlayerInfo.missionIndex.CompareTo((int)b.GetComponent<ABPlayerScript>().PlayerInfo.missionIndex); });
    }

    private void Start()
    {
       
    }

    // Spawn rulebook objects into game 
    public void SpawnRules()
    {
        for (int i = 0; i < rules.Length; i++)
        {
            GameObject spawn = Instantiate(rulesPrefab, GetLocation(), Quaternion.identity, transform);
            spawn.GetComponent<RulesScript>().RuleInfo = rules[i];

            rulesPrefab.gameObject.tag = "Book" + i;
            if (rules[i].isCorrect)
                amtOfCorrectRules++;
        }       
    }

    // Return specified Question Info object
    public SO_QuestionInfo GetQuestion(int inIndex)
    {
        return questions[inIndex];
    }


    // Obtain random location to assign rulebook objects
    private Vector3 GetLocation()
    {
        int pos = UnityEngine.Random.Range(0, spawnLocations.Length);
        if (spawnLocations[pos] != null)
        {
            Vector3 position = spawnLocations[pos].position;
            spawnLocations[pos] = null;
            return position;
        }
        else
            return GetLocation();
    }

    // Spawn player related to current level into the game world
    public void SpawnPlayer()
    {
        StartCoroutine(PlayerSpawn());
    }

    // Coroutine to complete player spawn actions
    IEnumerator PlayerSpawn()
    {
        yield return new WaitForSeconds(0.0f);

        GameObject spawn = Instantiate(playerCharacters[nextPlayer]);
        spawn.GetComponent<PathCreation.PathFollower>().pathCreator = SpawnManager.Instance.GetRandomPedestrianPath();
        spawn.GetComponent<ABPlayerScript>().SwitchComponents(false);

        // Increment index to load next level's player when ready
        nextPlayer++;
 
        MainManager.Instance.SetState(EGameState.PLAYER_START, spawn.GetComponent<ABPlayerScript>());
    }
}
