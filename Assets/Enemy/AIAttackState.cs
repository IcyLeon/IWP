using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAttackState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NavMeshAgent navMesh = animator.GetComponent<NavMeshAgent>();
        BaseEnemy baseEnemy = animator.GetComponent<BaseEnemy>();
        if (baseEnemy)
        {
            if (navMesh != null)
            {
                navMesh.enabled = false;
            }
            baseEnemy.SetisAttacking(true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NavMeshAgent navMesh = animator.GetComponent<NavMeshAgent>();
        BaseEnemy baseEnemy = animator.GetComponent<BaseEnemy>();
        if (baseEnemy)
        {
            baseEnemy.SetisAttacking(false);
        }
        if (navMesh != null)
        {
            navMesh.enabled = true;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
