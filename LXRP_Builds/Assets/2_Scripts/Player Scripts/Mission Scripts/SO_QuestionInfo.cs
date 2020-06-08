using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Question 1", menuName = "ScriptableObjects/QuestionInfo", order = 2)]
public class SO_QuestionInfo : ScriptableObject
{
    public int questionNo;
    public string questionText;
    public string aOption;
    public string bOption;
    public string cOption;
    public bool isCorrect;
    public char answer;
    public string correctText;
    public string incorrectText;
    public string InformationText;
    public Sprite questionImage;

    //Audio files for question, right and wrong answer
    public AudioClip question;
    public AudioClip rightAns;
    public AudioClip wrongAns;

    private bool isSelected = false;
    public bool IsSelected { get => isSelected; set => isSelected = value; }
}

