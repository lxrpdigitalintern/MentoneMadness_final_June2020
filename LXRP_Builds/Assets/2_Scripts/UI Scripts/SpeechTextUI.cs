// Utility class to animate text display
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class SpeechTextUI : MonoBehaviour
{
    // Intro Character Speech UI members
    [SerializeField] GameObject speechUI = null;
    [SerializeField] Button introSpeechNext = null;
    [SerializeField] TextMeshProUGUI introSpeechText = null;
    [SerializeField] Image introSpeechPortrait = null;

    [Space]
    [SerializeField] float typeSpeed = 0.09f;
    
    private int index;
    private int introIndex = 0;
    private string[] speechArray;
    private bool isTalking;

    public AudioClip[] introclip_array = null;
    AudioSource audioSource = null;
    Animator animatorPortrait = null;

    //[SerializeField] AudioClip[] introClips;
    private void Awake()
    {
        audioSource = speechUI.GetComponent<AudioSource>();
        animatorPortrait = speechUI.GetComponent<Animator>();

        introSpeechNext.onClick.AddListener(NextSentence);
    }

    // Initiate text display animation
    public void StartIntroSpeech(string[] speechText, Sprite characterPortrait, int inStartIndex, AudioClip[] introclip)
    {
        index = inStartIndex;
        introSpeechPortrait.sprite = characterPortrait;
        speechArray = speechText;
        introclip_array = introclip;
        introSpeechText.text = "";
        isTalking = false;
        animatorPortrait.SetBool("isTalking", isTalking);

        speechUI.SetActive(true);

        //StartCoroutine(Talk());
        StartCoroutine(Talk());
    }

    // Obtain next element of text array
    private void NextSentence()
    {
        if (isTalking)
            return;      

        if (index < speechArray.Length - 1)
        {
            index++;
            introSpeechText.text = "";

            //StartCoroutine(Talk());
            StartCoroutine(Talk());
            // Play Animation
        }
        else
        {
            introSpeechText.text = "";
            speechUI.SetActive(false);
        }
    }

    IEnumerator Talk()
    {
        isTalking = true;
        //Play sound
        animatorPortrait.SetBool("isTalking", isTalking);

        audioSource.clip = introclip_array[index];
        audioSource.Play();
        foreach (char letter in speechArray[index].ToCharArray())
        {
            introSpeechText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTalking = false;
        //introIndex++;
        animatorPortrait.SetBool("isTalking", isTalking);
        // Player animation
    }

    /*
    // Conduct "talking" animation and display actions
    IEnumerator Talk()
    {
        isTalking = true;
        //Play sound
        audioSource.Play();
        animatorPortrait.SetBool("isTalking", isTalking);

        foreach (char letter in speechArray[index].ToCharArray())
        {
            introSpeechText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTalking = false;
        animatorPortrait.SetBool("isTalking", isTalking);
        // Player animation
    }
    */

    // Remove listener on IntroSpeechNext button
    private void OnDestroy()
    {
        if(introSpeechNext != null)
            introSpeechNext.onClick.RemoveListener(NextSentence);
    }

}
