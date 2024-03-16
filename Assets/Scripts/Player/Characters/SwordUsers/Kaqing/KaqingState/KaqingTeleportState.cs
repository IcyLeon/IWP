using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KaqingTeleportState : KaqingElementalSkillState
{
    private float Range;
    private Vector3 TargetPosition, CurrentPosition;

    public KaqingTeleportState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        Range = GetKaqingState().KaqingData.ESkillRange;
        CurrentPosition = Kaqing.GetPointOfContact();

        Kaqing.GetPlayerManager().GetCharacterRB().useGravity = false;
        Kaqing.GetModel().SetActive(false);
        Kaqing.GetKaqingAimController().LookAtAimTarget();
    }

    public override void Update()
    {
        base.Update();
        UpdateTravelling();
    }

    private void UpdateTravelling()
    {
        Vector3 dir = (GetTargetPosition() - CurrentPosition);
        Range = Mathf.Min(dir.magnitude, GetKaqingState().KaqingData.ESkillRange);
        TargetPosition = (Range * dir.normalized) + CurrentPosition;
        TargetPosition.y = TargetPosition.y - Kaqing.GetCharacterHeight() / 2;

        if ((Kaqing.GetPointOfContact() - TargetPosition).magnitude >= 1.5f)
        {
            Kaqing.GetPlayerManager().GetCharacterRB().position = Vector3.MoveTowards(
                Kaqing.GetPlayerManager().GetCharacterRB().position, TargetPosition, Time.deltaTime * 40f
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
        Vector3 dir = GetKaqingState().KaqingData.kaqingTeleporter.transform.position - Kaqing.GetPointOfContact();

        if (Physics.Raycast(Kaqing.GetPointOfContact(), dir.normalized, out RaycastHit hit, Range, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return GetKaqingState().KaqingData.kaqingTeleporter.transform.position;
    }

    public override void Exit()
    {
    }
}
