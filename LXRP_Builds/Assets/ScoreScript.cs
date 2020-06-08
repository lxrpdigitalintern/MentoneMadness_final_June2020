using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ScoreScript : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
       animator = GetComponent<Animator>();    
    }

    public void ResetAnimParams()
    {
        animator.SetBool("isScoreEvent", false);
    }
}
