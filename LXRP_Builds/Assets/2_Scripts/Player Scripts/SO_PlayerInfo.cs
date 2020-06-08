// Class to respresent Player scriptable object 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo 1", menuName = "ScriptableObjects/CharacterInfo", order = 2)]
public class SO_PlayerInfo : ScriptableObject
{
    public EMissionType characterMission;
    public int missionIndex;
    public GameObject attachedObject;
    public float X, Y, Z;

    [Space]
    public Sprite portraitImage;
    public Sprite fullImage;
    public Sprite collectibleImage;

    [Space]
    public string characterName;
    public int instructionsIndex;
    [TextArea]
    public string[] instructionsSpeechText;

    public AudioClip[] introClip;
    [TextArea]
    public string objectivesText;
    [TextArea]
    public string[] introSpeechText;
}

