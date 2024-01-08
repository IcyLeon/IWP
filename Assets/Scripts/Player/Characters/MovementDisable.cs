using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDisable : StateMachineBehaviour
{
    public enum Lock
    {
        Enable,
        Disable
    }
    private PlayerCharacters playerCharacters;
    [SerializeField] Lock lockMovement;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerCharacters = animator.GetComponent<PlayerCharacters>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerCharacters == null)
            return;

        if (playerCharacters.GetPlayerManager() == null)
            return;

        if (playerCharacters.GetPlayerManager().GetPlayerController())
        {
            switch (lockMovement)
            {
                case Lock.Enable:
                    playerCharacters.GetPlayerManager().GetPlayerController().SetLockMovemnt(LockMovement.Enable);
                    break;
                case Lock.Disable:
                    playerCharacters.GetPlayerManager().GetPlayerController().SetLockMovemnt(LockMovement.Disable);
                    break;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //   
    //}

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
