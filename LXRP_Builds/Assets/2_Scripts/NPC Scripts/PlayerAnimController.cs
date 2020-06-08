using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimController : MonoBehaviour
{
    private Animator animator;
    const float locomotionAnimationSmoothTime = .1f;

    private NavMeshAgent agent;
    public NavMeshAgent NavMeshAgent { get => agent; set => agent = value; }

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CheckReferences();
    }

    private void CheckReferences()
    {
        if(NavMeshAgent == null)
        {
            Debug.Log("CustomCharacterController: Reference missing - 'agent'");
        }
        else if(animator == null)
        {
            Debug.Log("CustomCharacterController: Reference missing - 'animator'");
        }
    }

    private void OnDisable()
    {
        animator.SetFloat("speedPercent", 0, 0, 0);
        NavMeshAgent.isStopped = true;
    }

    private void OnEnable()
    {
        NavMeshAgent.isStopped = false;
    }

    void Update()
    {
        float speedPercent = NavMeshAgent.velocity.magnitude / NavMeshAgent.speed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);

        if (NavMeshAgent.remainingDistance <= 0.001)
            TargetLocation.Instance.Disable();
    }
}
