using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingControlState : IPlayerCharactersState
{
    private KaqingState kaqingState;

    public KaqingControlState(KaqingState kaqingState)
    {
        this.kaqingState = kaqingState;
    }

    public KaqingState GetKaqingState()
    {
        return kaqingState;
    }

    protected void StartAnimation(string animationString)
    {
        Animator animator = GetKaqingState().GetKaqing().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, true);
    }

    protected void StopAnimation(string animationString)
    {
        Animator animator = GetKaqingState().GetKaqing().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, false);
    }

    protected bool CanTriggerESlash()
    {
        if (GetKaqingState().KaqingData.kaqingTeleporter == null)
            return false;

        return !GetKaqingState().KaqingData.kaqingTeleporter.GetEnergyOrbMoving();
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
