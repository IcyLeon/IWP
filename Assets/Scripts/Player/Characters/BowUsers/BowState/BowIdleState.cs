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
        GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0);
        threasHold_Charged = 0;
    }

    public override void Update()
    {
        base.Update();
        UpdateBowAimThresHold();
    }

    public override void ChargeHold()
    {
        if (GetBowCharactersState().GetBowCharacters().GetPlayerManager().IsDashing() || GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
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
        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing() || GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

        GetBowCharactersState().GetBowCharacters().LaunchBasicAttack();
        threasHold_Charged = 0;
    }

    private void UpdateBowAimThresHold()
    {
        if (!GetBowCharactersState().GetBowCharacters().GetPlayerManager().CanPerformAction() || GetBowCharactersState().GetBowCharacters().GetBurstActive())
            return;

        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing() || GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
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
