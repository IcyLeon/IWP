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

    public virtual void OnAnimationTransition()
    {
    }
    private void InterruptUpdate()
    {
        if (!GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing())
            return;

        if (!GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

        switch (GetPlayerCharacterState())
        {
            case BowCharactersState bcs:
                GetPlayerCharacterState().ChangeState(bcs.bowIdleState);
                break;
            case SwordCharacterState scs:
                GetPlayerCharacterState().ChangeState(scs.swordIdleState);
                break;
        }
    }
    public virtual void Update()
    {
        InterruptUpdate();
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
