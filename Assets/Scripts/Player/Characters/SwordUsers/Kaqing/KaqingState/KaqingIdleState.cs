using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingIdleState : KaqingControlState
{
    private float threasHold_Charged;
    public KaqingIdleState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        threasHold_Charged = 0f;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ElementalSkillHold()
    {
        if (GetKaqingState().KaqingData.kaqingTeleporter != null)
            return;

        if (threasHold_Charged > 0.1f)
        {
            GetKaqingState().ChangeState(GetKaqingState().kaqingAimState);
            return;
        }
        threasHold_Charged += Time.deltaTime;
    }

    public override void ElementalSkillTrigger()
    {
        if (CanTriggerESlash())
        {
            GetKaqingState().ChangeState(GetKaqingState().kaqingTeleportState);
            return;
        }
        if (GetKaqingState().KaqingData.kaqingTeleporter == null)
        {
            GetKaqingState().GetKaqing().InitElementalSkillHitPos_NoAim();
            GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
        }
    }

    public override void ElementalBurstTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingBurstState);
    }
}
