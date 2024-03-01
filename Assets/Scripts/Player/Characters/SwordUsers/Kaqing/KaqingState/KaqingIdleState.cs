using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 forward, hitPos;
        Kaqing kaqing = GetKaqingState().GetKaqing();
        if (kaqing.GetNearestIDamage() == null)
        {
            forward = kaqing.transform.forward;
            forward.y = 0;
            forward.Normalize();
            hitPos = kaqing.GetRayPosition3D(kaqing.GetPlayerManager().GetPlayerOffsetPosition().position, forward, kaqing.GetKaqingState().KaqingData.ESkillRange);
        }
        else
        {
            forward = kaqing.GetNearestIDamage().GetPointOfContact() - kaqing.GetPlayerManager().GetPlayerOffsetPosition().position;
            forward.Normalize();
            hitPos = kaqing.GetRayPosition3D(kaqing.GetPlayerManager().GetPlayerOffsetPosition().position, forward, kaqing.GetKaqingState().KaqingData.ESkillRange);
        }
        kaqing.GetKaqingThrowTeleporter().SetElementalHitPosition(hitPos);
    }

    public override void ElementalBurstTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingBurstState);
    }
}
