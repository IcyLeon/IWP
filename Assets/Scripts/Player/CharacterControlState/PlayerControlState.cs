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

    public BowCharactersState GetBowCharactersState()
    {
        return (BowCharactersState)GetPlayerCharacterState();
    }
    public SwordCharacterState GetSwordCharactersState()
    {
        return (SwordCharacterState)GetPlayerCharacterState();
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
    public virtual void LateUpdate()
    {
    }
    public virtual void OnAnimationTransition()
    {
    }
    private void InterruptUpdate()
    {
        if (!GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDeadState())
            return;

        switch (GetPlayerCharacterState())
        {
            case BowCharactersState bcs:
                if (bcs.GetCurrentState() is not BowIdleState)
                    bcs.ChangeState(bcs.bowIdleState);
                break;
            case SwordCharacterState scs:
                if (scs.GetCurrentState() is not SwordIdleState)
                    scs.ChangeState(scs.swordIdleState);
                break;
        }
    }
    public virtual void Update()
    {
        InterruptUpdate();
        UpdateBasicAttacks();
    }

    private void UpdateBasicAttacks()
    {
        if (Time.time - playerCharacterState.CommonCharactersData.LastClickedTime >= 1f)
        {
            playerCharacterState.ResetBasicAttacks();
        }
    }

    protected bool HasReachedEndOfBasicAttackAnimation()
    {
        string AtkName = "Attack" + playerCharacterState.CommonCharactersData.MaxAttackPhase;
        return playerCharacterState.GetPlayerCharacters().GetAnimator().GetBool(AtkName) || playerCharacterState.CommonCharactersData.BasicAttackPhase >= playerCharacterState.CommonCharactersData.MaxAttackPhase;
    }

    protected virtual void LaunchBasicAttack()
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

    public virtual void UpdateElementalSkill()
    {
    }

    public virtual void UpdateBurst()
    {
    }
}
