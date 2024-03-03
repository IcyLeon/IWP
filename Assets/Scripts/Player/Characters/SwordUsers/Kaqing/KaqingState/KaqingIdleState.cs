using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class KaqingIdleState : SwordIdleState
{
    private float threasHold_Charged;
    public KaqingIdleState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public KaqingState GetKaqingState()
    {
        return (KaqingState)GetPlayerCharacterState();
    }

    protected bool CanTriggerESlash()
    {
        if (GetKaqingState().KaqingData.kaqingTeleporter == null)
            return false;

        return !GetKaqingState().KaqingData.kaqingTeleporter.GetEnergyOrbMoving();
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

        if (threasHold_Charged > 0.25f)
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
            ElementalSkillHitPos_NoAim();
            GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
        }
    }

    private void ElementalSkillHitPos_NoAim()
    {
        Vector3 dir = GetKaqingState().GetKaqing().LookAtClosestTarget();
        Vector3 hitpos = GetKaqingState().GetKaqing().GetRayPosition3D(GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerOffsetPosition().position, dir, GetKaqingState().GetKaqing().GetKaqingState().KaqingData.ESkillRange);
        GetKaqingState().GetKaqing().GetKaqingAim().SetAimTargetPosition(hitpos);
    }

    public override void ElementalBurstTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingBurstState);
    }
}
