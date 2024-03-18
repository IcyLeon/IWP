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

    public BowCharacterState GetBowCharactersState()
    {
        return (BowCharacterState)GetPlayerCharacterState();
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
    protected virtual void DeadUpdate()
    {
        if (!GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDeadState())
            return;

        if (GetPlayerCharacterState().GetCurrentState() is not CharacterDeadState)
            GetPlayerCharacterState().ChangeState(GetPlayerCharacterState().characterDeadState);
    }
    protected virtual void TransitionToAttackState()
    {

    }
    public virtual void Update()
    {
        DeadUpdate();
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
