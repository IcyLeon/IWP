using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingIdleState : SwordIdleState
{
    protected Kaqing Kaqing;
    private float threasHold_Charged;
    public KaqingIdleState(PlayerCharacterState pcs) : base(pcs)
    {
        Kaqing = GetKaqingState().GetKaqing();
    }

    protected KaqingState GetKaqingState()
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

        if (threasHold_Charged > 0.2f)
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
        Vector3 dir = Kaqing.LookAtClosestTarget();
        Vector3 hitpos = PlayerController.GetRayPosition3D(Kaqing.GetPointOfContact(), dir, GetKaqingState().KaqingData.ESkillRange);
        Kaqing.GetKaqingAimController().SetAimTargetPosition(hitpos);
    }

    public override void ElementalBurstTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingBurstState);
    }
}
