using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDisable : StateMachineBehaviour
{
    public enum LockMovement
    {
        Enable,
        Disable
    }

    [SerializeField] LockMovement lockMovement;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCharacters playerCharacters = animator.GetComponent<PlayerCharacters>();
        if (playerCharacters.GetPlayerController())
        {
            PlayerController playerController = playerCharacters.GetPlayerController();
            switch(lockMovement)
            {
                case LockMovement.Enable:
                    playerCharacters.GetPlayerController().SetLockMovemnt(PlayerController.LockMovement.Enable);
                    break;
                case LockMovement.Disable:
                    playerCharacters.GetPlayerController().SetLockMovemnt(PlayerController.LockMovement.Disable);
                    break;
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
