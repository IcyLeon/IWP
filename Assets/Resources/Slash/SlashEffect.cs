using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffect : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    [SerializeField] float Delay;
    private bool isSlash = false;
    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SwordCharacters characters = animator.transform.GetComponent<SwordCharacters>();

        if (stateInfo.normalizedTime >= Delay && !isSlash)
        {
            characters.SetAttackPhase(characters.GetBasicAttackPhase() + 1);
            GameObject slash = AssetManager.GetInstance().SpawnSlashEffect(characters.GetEmitterPivot().position, characters.GetEmitterPivot().rotation);
            slash.GetComponentInChildren<Sword>().SetSwordCharacterWield(characters);
            isSlash = true;
        }
    } 
    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isSlash = false;
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
