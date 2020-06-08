using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SpeechTextUI))]
[RequireComponent(typeof(QuestionUIManager))]
public class UIManager : MonoBehaviour
{
    // constants
    private int MAX_NUM_RULEBOOKS = 5;

    // Singleton Members
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }
    ABPlayerScript UIcurrSelectedPlayer;
    public ABPlayerScript UICurrSelectedPlayer { get => UIcurrSelectedPlayer; }
    // Level status text components
    [SerializeField] TextMeshProUGUI scoreText = null;
    [SerializeField] TextMeshProUGUI LevelStatus = null;
    [SerializeField] TextMeshProUGUI GameOver = null;
    TextMeshProUGUI[] TextElements = null;
        private Animator animatorScoreText = null;

    [SerializeField] Button resetButton = null;
    [SerializeField] Button backToMenuButton = null;
    [SerializeField] Button exitButton = null;                      //change : Exit button 
    [SerializeField] Button endGameOKButton = null;                 // End game ok button
    // Character Info UI members
    [Space]
    [SerializeField] Text characterNameTxt = null;
    [SerializeField] Image portraitImage = null;
    [SerializeField] Image fullImage = null;
    [SerializeField] Image collectibleImage = null;
    [SerializeField] Text objectiveText = null;
    [SerializeField] Text updateText = null;

    // Rule UI Members
    [Space]
    [SerializeField] GameObject ruleBookUI = null;
    [SerializeField] Toggle ruleSelectionTrue = null;
    [SerializeField] Toggle ruleSelectionFalse = null;
    [SerializeField] Text ruleText = null;
    [SerializeField] Text ruleNo = null;
    [SerializeField] Button ruleOkButton = null;
    private SO_RuleInfo ruleInfo = null;
    private int numRulebooksCollected = 0;

    // Question UI Members
    [Space]
    private QuestionUIManager questionComponent = null;
    [SerializeField] Button aButton = null;
    [SerializeField] Button bButton = null;
    [SerializeField] Button cButton = null;
    [SerializeField] Button okButton = null;
    [SerializeField] List<GameObject> donuts = new List<GameObject>();
    [SerializeField] Button starButton = null;
    [SerializeField] List<GameObject> starImages = new List<GameObject>();
    [SerializeField] Button starImageCloseButton = null;
    private int currentQuestionIndex;
    private const int NUM_QUESTIONS = 5;
    public bool AllDonutsCollected = false;

    // Redner Images
    [SerializeField] GameObject renderImage1;
    [SerializeField] GameObject renderImage2;
    [SerializeField] GameObject renderImage3;
    [SerializeField] GameObject renderImage4;
    [SerializeField] GameObject renderImage5;

    //Level2 prefabs on and off
    [SerializeField] public GameObject Level2Container = null;

    //Sidebar set up according to levels
    [SerializeField] public GameObject Button1 = null;
    [SerializeField] public GameObject Button2 = null;
    [SerializeField] public GameObject Button3 = null;
    [SerializeField] public GameObject Button4 = null;
    [SerializeField] public GameObject Button5 = null;

    //Level1 Arrows
    [SerializeField] public GameObject Level1greenArrows = null;

    // End game UI
    [SerializeField] public GameObject endGameComponent;
    [SerializeField] TextMeshProUGUI finalScore;
    GameObject infoHub;

    // Spech UI components
    private SpeechTextUI speechTextComponent;
    [SerializeField] GameObject smallMenu = null;
    private bool levelTextSet = false;

    //Audio play for rule Book
    public AudioSource ruleAudio = null;
    public AudioSource AnsAudio = null;
    public AudioSource DonutAudio = null;
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

        speechTextComponent = GetComponent<SpeechTextUI>();
        questionComponent = GetComponent<QuestionUIManager>();
        //ruleAudio = this.GetComponent<AudioSource>();             //Audio source to play question and right answer
        AnsAudio = ruleBookUI.GetComponent<AudioSource>();
        //DonutAudio = questionComponent.GetComponent<AudioSource>();
    }
    private void Start()
    {
        if (!CheckMissingRefs())
            return;

        animatorScoreText = scoreText.gameObject.GetComponent<Animator>();

        // Add listeners and delegate function to UIManager components
        resetButton.onClick.AddListener(OnResetButtonClicked);
        backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);                    //change : Exit button
        endGameOKButton.onClick.AddListener(OnEndGameOKButtonClicked);          //End game UI

        ruleSelectionTrue.onValueChanged.AddListener(OnRuleSelectionTrue);
        ruleSelectionFalse.onValueChanged.AddListener(OnRuleSelectionFalse);
        ruleOkButton.onClick.AddListener(OnRuleOKClicked);
        aButton.onClick.AddListener(OnQuestionAClicked);
        bButton.onClick.AddListener(OnQuestionBClicked);
        cButton.onClick.AddListener(OnQuestionCClicked);
        okButton.onClick.AddListener(OnOkButtonClicked);
        starButton.onClick.AddListener(OnStarButtonClicked);
        starImageCloseButton.onClick.AddListener(OnStarButtonCloseClicked);

        TextElements = FindObjectsOfType<TextMeshProUGUI>();
    }

    // Make "infoHub" object outline appear when all donuts have been collected during Level 3
    public void ShowInfoHub()
    {
        infoHub = GameObject.FindGameObjectWithTag("InfoHub");
        infoHub.GetComponent<Outline>().OutlineWidth = 7;
    }

    // Actions when "Star" button on QuestionUI is clicked
    private void OnStarButtonClicked()
    {
        currentQuestionIndex = questionComponent.GetCurrentQuestionIndex();

        ShowRender(currentQuestionIndex);
        HideQuestionUI();
        if (questionComponent.numberOfQuestionsAnswered == NUM_QUESTIONS)
        {
            AllDonutsCollected = true;
            ShowInfoHub();
        }
    }

    // Reveal specified render image within game world "above" level map
    private void ShowRender(int inIndex)
    {
        switch(inIndex)
        {
            case 0:
                renderImage1.gameObject.SetActive(true);
                break;
            case 1:
                renderImage2.gameObject.SetActive(true);
                break;
            case 2:
                renderImage3.gameObject.SetActive(true);
                break;
            case 3:
                renderImage4.gameObject.SetActive(true);
                break;
            case 4:
                renderImage5.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    // Redundant function?
    private void OnStarButtonCloseClicked()
    {
        DonutAudio.Stop();
        starImages[currentQuestionIndex].SetActive(false);
        starImageCloseButton.gameObject.SetActive(false);
    }

    // Delegate function when "True" check box is checked
    private void OnRuleSelectionTrue(bool isSlected)
    {
        MainManager.Instance.OnRuleSelect(isSlected, ruleInfo, true);
    }

    // Delegate function when "False" check box is checked
    private void OnRuleSelectionFalse(bool isSlected)
    {
        MainManager.Instance.OnRuleSelect(isSlected, ruleInfo, false);
    }

    // Function to determine all 5 rulebooks have been collected
    public bool AllRuleBooksCollected()
    {
        return numRulebooksCollected == MAX_NUM_RULEBOOKS;
    }

    // Alter rulebook UI with text that shows result of check box seletion
    public void UpdateRulebookText(string inText)
    {
        ruleText.text = inText;
        ruleSelectionFalse.gameObject.SetActive(false);
        ruleSelectionTrue.gameObject.SetActive(false);
        ruleOkButton.gameObject.SetActive(true);
    }

    // Delegate function to run when 'OK' button on rule book UI is clicked
    // Contains trigger end of Level 2 check
    public void OnRuleOKClicked()
    {
        ruleText.text = "";
        ResetRulebookUI();
        ruleBookUI.gameObject.SetActive(false);
        int collectedRule = MainManager.Instance.GetNumSelectedRules();
        if (collectedRule == MAX_NUM_RULEBOOKS)
            MainManager.Instance.SetState(EGameState.QUEST_COMPLETE);
    }

    // Restore rulebook UI to be ready to display next True/False scenario
    private void ResetRulebookUI()
    {
        ruleOkButton.gameObject.SetActive(false);
        ruleSelectionTrue.gameObject.SetActive(true);
        ruleSelectionTrue.isOn = false;
        ruleSelectionFalse.gameObject.SetActive(true);
        ruleSelectionFalse.isOn = false;
        starButton.gameObject.SetActive(false);
    }

    // Delegate function when 'A' button is clicked
    private void OnQuestionAClicked()
    {
        Debug.Log("A Clicked");
        questionComponent.OnQuestionButtonClicked('A');
    }

    // Delegate function when 'B' button is clicked
    private void OnQuestionBClicked()
    {
        Debug.Log("B Clicked");
        questionComponent.OnQuestionButtonClicked('B');
    }

    // Delegate function when 'C' button is clicked
    private void OnQuestionCClicked()
    {
        Debug.Log("C Clicked");
        questionComponent.OnQuestionButtonClicked('C');
    }

    // Delegate function to "hide" QuestionUI when OK button is clicked
    private void OnOkButtonClicked()
    {
        HideQuestionUI();
    }

    // Removes Listener from component objects
    private void OnDestroy()
    {
        if (resetButton != null)
            resetButton.GetComponent<Button>().onClick.RemoveListener(OnResetButtonClicked);

        if (backToMenuButton != null)
            backToMenuButton.GetComponent<Button>().onClick.RemoveListener(OnBackToMenuButtonClicked);

        if (ruleSelectionTrue != null)
            ruleSelectionTrue.GetComponent<Toggle>().onValueChanged.RemoveListener(OnRuleSelectionTrue);

        if (ruleSelectionFalse != null)
            ruleSelectionFalse.GetComponent<Toggle>().onValueChanged.RemoveListener(OnRuleSelectionFalse);
    }

    #region MainMenuUI Methods

    // Enact reset game actions when selected
    private void OnResetButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Enact back to menu actions when selected
    private void OnBackToMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();                         //change : Quit on Exit button
    }

    private void OnEndGameOKButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    // Display intro/outro message when starting/ending a level
    public void DisplayLevelStatusMessage(EGameState inLevelState, EMissionType inMission)
    {
        if (!levelTextSet)
        {
            LevelStatus.gameObject.SetActive(true);
            levelTextSet = true;
        }
        int level = (int)inMission;

        if (inLevelState == EGameState.QUEST_START)
        {
            LevelStatus.SetText("Level " + ++level);
        }
        else if (inLevelState == EGameState.QUEST_COMPLETE)
        {
            LevelStatus.SetText("Level " + ++level + " Completed");
        }
    }

    // Obtain specified text element
    //public TextMeshProUGUI getTextElement(string inLabel)
    //{
    //    TextMeshProUGUI result = null;

    //    GameObject obj = GameObject.FindGameObjectWithTag(inLabel);
    //    result = obj.GetComponent<TextMeshProUGUI>();
    //    return result;
    //}

    // Initialise "Game Over" message, set to invisible
    public void InitGameOverMessage()
    {
        GameOver.gameObject.SetActive(false);
    }
    
    // Display "Game Over" message for 3 seconds
    public void DisplayGameOverMessage()
    {
        GameOver.text = "Game Over!";
        GameOver.gameObject.SetActive(true);
        StartCoroutine(DoGameOverDelay());
    }

    // Display all in game world renders at game end
    private void ShowAllRenders()
    {
        renderImage1.gameObject.SetActive(true);
        renderImage2.gameObject.SetActive(true);
        renderImage3.gameObject.SetActive(true);
        renderImage4.gameObject.SetActive(true);
        renderImage5.gameObject.SetActive(true);
    }

    // Display "Game Finished" for 3 seconds
    public void DisplayGameFinishedMessage()
    {
        HideLevelStatusText();
        finalScore.text = MainManager.Instance.GetScore() + " points.";
        endGameComponent.gameObject.SetActive(true);
        ShowAllRenders();
    }

    // Coroutine to switch "Game Over" message off and show end game menu
    private IEnumerator DoGameOverDelay()
    {
        yield return new WaitForSeconds(3.0f);
        smallMenu.SetActive(true);
        GameOver.gameObject.SetActive(false);
    }

    // Initialise "Level" message, set to invisible
    public void HideLevelStatusText()
    {
        LevelStatus.gameObject.SetActive(false);
        LevelStatus.text = "level";
    }

    // Display "Level" message
    public void ShowLevelStatusText()
    {
        LevelStatus.gameObject.SetActive(true);
    }

    // Assign rule information to Rule Book UI components
    public void SetRuleInfo(SO_RuleInfo info)
    {
        ruleInfo = info;
        ruleNo.text = "#" + ruleInfo.ruleNo;
        ruleAudio.clip = ruleInfo.question;                     // audio clip for rule
        ruleAudio.Play();
        ruleText.text = ruleInfo.ruleText;
        Debug.Log(ruleText.text);
        ruleSelectionTrue.isOn = ruleInfo.IsSelected;
        //StartCoroutine(playRuleAudio(info));
        ResetRulebookUI();
        ruleBookUI.SetActive(true);
    }

    //Co-routine for playing rule book audio
   /* IEnumerator playRuleAudio(SO_RuleInfo info)
    {
        ruleAudio.clip = info.question;                     // audio clip for rule
        ruleAudio.Play();
        yield return new WaitForSeconds(ruleAudio.clip.length);
    } */

    // Assign player information to current character scomponents
    public void SetPlayerInfo(SO_PlayerInfo info)
    {
        characterNameTxt.text = info.characterName;
        fullImage.sprite = info.fullImage;
        collectibleImage.sprite = info.collectibleImage;
        objectiveText.text = info.objectivesText;
       if (characterNameTxt.text == "Construction Worker")
        {
            portraitImage.sprite = info.portraitImage;
            UpdateRules(0);
        }
       else if(characterNameTxt.text == "Hotdog Vendor")
        {
            portraitImage.sprite = info.portraitImage;
            updateText.text = "Follow the green arrow";
        }
        else if (characterNameTxt.text == "School Girl")
        {
            //portraitImage.sprite = UIManager.Instance.schoolportraitImage;
            portraitImage.sprite = info.portraitImage;
            updateText.text = "Look up once all the donuts are collected";
        }

    }

    // Enact animations and display for instructions display
    public void StartIntroSpeech(string[] speechText, Sprite characterPortrait, int startIndex, AudioClip[] introclip)
    {
        speechTextComponent.StartIntroSpeech(speechText, characterPortrait, startIndex, introclip);
    }

    // Sets update rule text
    public void UpdateRules(int no)
    {
            string text = no + " / "
                + SpawnManager.Instance.getNoOfSpawns(ESpawnSelection.RULES)
                + " Rules Selected";
            updateText.text = text;
        //Debug.Log("Rule is" + text);
    }

    // Runs updated score animation and displays soecified score total
    public void UpdateScore(int score)
    {
        Debug.Log("UI Manager Update Score");
        scoreText.text = score.ToString();
        animatorScoreText.SetBool("isScoreEvent", true);
    }

    // Make QuestionUI visible with current question
    public void ShowQuestionUI(int inIndex)
    {
        questionComponent.SetActive(true);
        questionComponent.SetCurrentQuestion(inIndex);
        SetScenarioText();
        HideDonut(inIndex);
    }

    // Hide Question UI
    public void HideQuestionUI()
    {
        questionComponent.SetActive(false);
    }

    // Assign question/scenario text component the current question text
    public void SetScenarioText()
    {
        questionComponent.SetScenarioText();
    }

    // Toggle function to display A, B, C buttons, options text and star button
    public void ShowHideABCButtons(bool toggle)
    {
        aButton.gameObject.SetActive(toggle);
        bButton.gameObject.SetActive(toggle);
        cButton.gameObject.SetActive(toggle);
        questionComponent.ShowOptionsText(toggle);
        starButton.gameObject.SetActive(!toggle);
    }

    // Make all "donut" objects visible at start of Level 3
     public void ShowDonuts(bool show)
    {
        foreach (GameObject d in donuts)
        {
            if(d!=null)                             //check if donut is destroyed or not
                d.gameObject.SetActive(show);
        }
    }

    // Make specified "donut" object inactive when collected
    public void HideDonut(int index)
    {
        //donuts[index].gameObject.SetActive(false);
        Destroy(donuts[index].gameObject);              //Destroy game object to avoid reappearance issue
    }

    // Ensure certain references are not null, notify of they are
    private bool CheckMissingRefs()
    {
        if (resetButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'resetButton'");
            return false;
        }
        if (backToMenuButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'backToMenuButton'");
            return false;
        }
        if (exitButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'ExitButton'");      //change : Exit button 
            return false;
        }

        if(endGameOKButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'ExitButton'");      //change : Exit button 
            return false;
        }
        return true;
    }
}
