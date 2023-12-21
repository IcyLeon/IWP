using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KaqingTeleportState : KaqingElementalSkillState
{
    private float Range;
    private Vector3 TargetPosition;

    public KaqingTeleportState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Range = GetKaqingState().KaqingData.ESkillRange;
        TargetPosition = GetTargetPosition();

        GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().GetCharacterRB().useGravity = false;
        //GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerController().GetPlayerState().ChangeState(
        //    GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerController().GetPlayerState().playerStayAirborneState
        //    );

        GetKaqingState().GetKaqing().LookAtElementalHitPos();
    }

    public override void Update()
    {
        base.Update();
        UpdateTravelling();
    }

    private void UpdateTravelling()
    {
        if ((GetKaqingState().GetKaqing().GetPlayerManager().GetCharacterRB().position - TargetPosition).magnitude > 1f)
        {
            GetKaqingState().GetKaqing().GetPlayerManager().GetCharacterRB().position = Vector3.MoveTowards(
                GetKaqingState().GetKaqing().GetPlayerManager().GetCharacterRB().position, TargetPosition, Time.deltaTime * 100f
                );
        }
        else
        {
            GetKaqingState().ChangeState(GetKaqingState().kaqingESlash);
            return;
        }
    }
    private Vector3 GetTargetPosition()
    {
        Vector3 dir = GetKaqingState().KaqingData.kaqingTeleporter.transform.position - GetKaqingState().GetKaqing().GetPointOfContact();

        if (Physics.Raycast(GetKaqingState().GetKaqing().GetPointOfContact(), dir.normalized, out RaycastHit hit, Range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }
        else
        {
            if (dir.magnitude >= Range)
                return GetKaqingState().GetKaqing().GetPointOfContact() + dir.normalized * Range;
        }

        return GetKaqingState().KaqingData.kaqingTeleporter.transform.position;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
