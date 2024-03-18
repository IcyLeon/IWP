using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowIdleState : BowControlState
{
    private float threasHold_Charged;

    public BowIdleState(PlayerCharacterState pcs) : base(pcs)
    {
        threasHold_Charged = 0;
    }

    public override void Enter()
    {
        base.Enter();
        threasHold_Charged = 0;
    }

    public override void Update()
    {
        base.Update();
        UpdateJumpState();
        UpdateBowAimThresHold();
    }

    private void UpdateJumpState()
    {
        if (!GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsJumping())
            return;

        GetBowCharactersState().ChangeState(GetBowCharactersState().bowJumpState);
    }

    public override void ChargeHold()
    {
        if (!CanAim())
            return;

        if (threasHold_Charged > 0.25f)
        {
            GetBowCharactersState().ChangeState(GetBowCharactersState().bowAimState);
            return;
        }
        threasHold_Charged += Time.deltaTime;
    }

    public override void ChargeTrigger()
    {
        if (!CanAim())
            return;

        LaunchBasicAttack();
        threasHold_Charged = 0;
    }

    protected override void LaunchBasicAttack()
    {
        if (Time.timeScale == 0)
            return;

        if (Time.time - GetBowData().LastClickedTime > CommonCharactersData.AttackRate)
        {
            TransitionToAttackState();
            GetBowData().LastClickedTime = Time.time;
        }
    }


    private bool CanAim()
    {
        if (!GetBowCharactersState().GetBowCharacters().GetPlayerManager().CanPerformAction() || GetBowCharactersState().GetBowCharacters().GetBurstActive())
            return false;

        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing() || GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDeadState())
            return false;

        return true;
    }
    private void UpdateBowAimThresHold()
    {
        if (!CanAim())
            return;

        if (Input.GetMouseButton(1))
        {
            GetBowCharactersState().ChangeState(GetBowCharactersState().bowAimState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        threasHold_Charged = 0;
    }
}
