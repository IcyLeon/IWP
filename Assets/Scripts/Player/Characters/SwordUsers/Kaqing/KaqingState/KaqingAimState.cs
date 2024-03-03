using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingAimState : KaqingElementalSkillState
{
    private float MaxTimer = 5f;
    private float Timer;
    public KaqingAimState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Timer = 0f;
        GetKaqingState().GetKaqing().GetSwordModel().gameObject.SetActive(false);
    }

    public override void ElementalSkillTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
    }

    private void ElementalSkillHitPos_Aim()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        Vector3 hitdir = (ray.origin + ray.direction * GetKaqingState().GetKaqing().GetKaqingState().KaqingData.ESkillRange) - GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerOffsetPosition().position;
        GetKaqingState().GetKaqing().GetKaqingAim().UpdateTargetOrb(GetKaqingState().GetKaqing().GetRayPosition3D(GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerOffsetPosition().position, hitdir, GetKaqingState().GetKaqing().GetKaqingState().KaqingData.ESkillRange));
    }

    public override void Update()
    {
        base.Update();

        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

        GetPlayerCharacterState().GetPlayerCharacters().UpdateCameraAim();
        ElementalSkillHitPos_Aim();

        if (Timer >= MaxTimer)
        {
            GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
            return;
        }
        Timer += Time.deltaTime;
    }

    public override void Exit()
    {
        GetKaqingState().GetKaqing().GetKaqingAim().DestroyTargetOrb();
    }
}
