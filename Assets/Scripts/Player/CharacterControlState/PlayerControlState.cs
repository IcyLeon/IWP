using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlState : IPlayerCharactersState
{
    private PlayerCharacterState playerCharacterState;

    public PlayerControlState(PlayerCharacterState pcs)
    {
        playerCharacterState = pcs;
    }
    public PlayerCharacterState GetPlayerCharacterState()
    {
        return playerCharacterState;
    }
    protected void StartAnimation(string animationString)
    {
        Animator animator = GetPlayerCharacterState().GetPlayerCharacters().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, true);
    }

    protected void StopAnimation(string animationString)
    {
        Animator animator = GetPlayerCharacterState().GetPlayerCharacters().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, false);
    }
    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnAnimationTransition()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void ChargeHold()
    {
    }

    public virtual void ChargeTrigger()
    {
    }

    public virtual void ElementalBurstTrigger()
    {

    }

    public virtual void ElementalSkillTrigger()
    {
    }

    public virtual void ElementalSkillHold()
    {
    }

    public virtual void UpdateOffline()
    {

    }
}
