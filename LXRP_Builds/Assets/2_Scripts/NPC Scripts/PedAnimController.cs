using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PathFollower))]
public class PedAnimController : MonoBehaviour
{
    Animator animator = null;
    const float locomotionAnimationSmoothTime = .1f;

    [SerializeField] float[] speedArray = { 0.1f, 0.2f, 0.15f ,0.3f };

    PathFollower pathFollower;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pathFollower = GetComponent<PathFollower>();
    }

    private void Start()
    {
        // Set a random speed
        pathFollower.speed = speedArray[Random.Range(0, speedArray.Length)] ;
    }

    void Update()
    {
        if (pathFollower.isActiveAndEnabled)
        {
            animator.SetFloat("speedPercent", pathFollower.speed, locomotionAnimationSmoothTime, Time.deltaTime);
        }
    }
}
