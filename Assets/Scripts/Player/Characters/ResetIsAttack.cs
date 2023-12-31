using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsAttack : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCharacters playerCharacters = animator.GetComponent<PlayerCharacters>();
        if (playerCharacters != null)
        {
            if (Characters.ContainsParam(animator, "isDashing"))
            {
                animator.SetBool("isDashing", false); // fix the issue from dash > attack immediately
            }
            playerCharacters.SetisAttacking(true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    PlayerCharacters playerCharacters = animator.GetComponent<PlayerCharacters>();
    //    if (playerCharacters != null)
    //        playerCharacters.SetisAttacking(true);
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(layerIndex))
            return;

        if (animator.GetNextAnimatorStateInfo(layerIndex).IsTag("ATK"))
            return;

        PlayerCharacters playerCharacters = animator.GetComponent<PlayerCharacters>();
        if (playerCharacters != null)
        {
            if (!Characters.ContainsParam(animator, "NextAtk"))
            {
                playerCharacters.ResetAttack();
            }
            else
            {
                if (animator.GetBool("NextAtk"))
                {
                    playerCharacters.ResetAttack();
                }
            }
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
