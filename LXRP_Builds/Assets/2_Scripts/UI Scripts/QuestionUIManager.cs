// Manager class to handle interactions and actions for Level 3 question answering
using UnityEngine;
using TMPro;

public class QuestionUIManager : MonoBehaviour
{
    // constants
    private const char ANSWER_A = 'A';
    private const char ANSWER_B = 'B';
    private const char ANSWER_C = 'C';
    private const int MAX_NUM_QUESTIONS = 5;

    // Component objects
    [SerializeField] GameObject questionUI = null;
    [SerializeField] TextMeshProUGUI scenarioText = null;
    [SerializeField] TextMeshProUGUI scenarioNumberText = null;
    [SerializeField] int activeQuestionIndex = 0;
    [SerializeField] public int numberOfQuestionsAnswered = 0;
    private SO_QuestionInfo currentQuestion = null;
    [SerializeField] TextMeshProUGUI aOption;
    [SerializeField] TextMeshProUGUI bOption;
    [SerializeField] TextMeshProUGUI cOption;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Accessor to return index to question scriptable objects
    public int GetCurrentQuestionIndex()
    {
        return activeQuestionIndex;
    }

    // Set QuestioUI component to active or not
    public void SetActive(bool inActive)
    {
        questionUI.SetActive(inActive);
    }

    // Set text to display on QuestionUI and present A, B & C buttons
    public void SetScenarioText()
    {
        scenarioText.text = currentQuestion.questionText;
        scenarioNumberText.text = (currentQuestion.questionNo + 1).ToString();
        UIManager.Instance.DonutAudio.clip = currentQuestion.question;      //set and play current question audio clip
        UIManager.Instance.DonutAudio.Play();
        aOption.text = currentQuestion.aOption;
        bOption.text = currentQuestion.bOption;
        cOption.text = currentQuestion.cOption;
        aOption.gameObject.SetActive(true);
        UIManager.Instance.ShowHideABCButtons(true);
    }

    // Move index into question scriptable objects
    public void SetCurrentQuestion(int index)
    {
        activeQuestionIndex = index;
        GetCurrentQuestioInfo();
    }

    // Obtain full information (text, correct answer etc.) of current question
    private void GetCurrentQuestioInfo()
    {
        currentQuestion = SpawnManager.Instance.GetQuestion(activeQuestionIndex);
    }

    // Present current question information on QuestionUI
    public void DisplayCurrentQuestion()
    {
        questionUI.SetActive(true);
        scenarioText.text = currentQuestion.questionText;
        scenarioNumberText.text = (activeQuestionIndex + 1).ToString();
        
    }

    // function to handle when A, B or C button is clicked
    public void OnQuestionButtonClicked(char inButtonLetter)
    {
        if (currentQuestion.answer == inButtonLetter) // Correct letter selected
        {
            DisplayCorrectText(true);
            UIManager.Instance.DonutAudio.clip = currentQuestion.rightAns; //play audio for right answer
            UIManager.Instance.DonutAudio.Play();
            numberOfQuestionsAnswered++;
        }
        else // Incorrect letter selected
        {
            DisplayCorrectText(false);
            //UIManager.Instance.DonutAudio.Stop();
            UIManager.Instance.DonutAudio.clip = currentQuestion.wrongAns; //play audio for wrong answer
            UIManager.Instance.DonutAudio.Play();
            numberOfQuestionsAnswered++;
        }
    }

    // Display reponse text when correct answer is selected
    private void DisplayCorrectText(bool inCorrect)
    {
        // Hide A,B & C buttons
        UIManager.Instance.ShowHideABCButtons(false);

        if (inCorrect)
        {
            scenarioText.text = currentQuestion.correctText;
            MainManager.Instance.UpdateScore(EScoreEvent.CORRECT_QUESTION);
        }
        else
        {
            scenarioText.text = currentQuestion.incorrectText;
        }
        ShowOptionsText(false);
        scenarioText.text += "<br><br>" + currentQuestion.InformationText;
    }

    // Display specific A, B and C option text for a given question
    public void ShowOptionsText(bool toggle)
    {
        aOption.gameObject.SetActive(toggle);
        bOption.gameObject.SetActive(toggle);
        cOption.gameObject.SetActive(toggle);
    }
}
