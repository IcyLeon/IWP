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
        Kaqing.GetPlayerManager().GetPlayerController().DisableInput(Kaqing.GetPlayerManager().GetPlayerController().GetPlayerActions().Move, this is KaqingAimState);
        Kaqing.ToggleAimCamera(true);
        Kaqing.PlayAimSound(true);
        Kaqing.GetSwordModel().gameObject.SetActive(false);
    }

    public override void ElementalSkillTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
        return;
    }

    private void ElementalSkillHitPos_Aim()
    {
        Vector3 EmitterPosition = Kaqing.GetPointOfContact();
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        Vector3 hitdir = (ray.origin + ray.direction * (GetKaqingState().KaqingData.ESkillRange + GetOffSet(EmitterPosition))) - EmitterPosition;
        Kaqing.GetKaqingAimController().UpdateTargetOrb(PlayerController.GetRayPosition3D(EmitterPosition, hitdir, GetKaqingState().KaqingData.ESkillRange));
    }

    private float GetOffSet(Vector3 EmitterPos)
    {
        return Mathf.Sqrt((Camera.main.transform.position - EmitterPos).magnitude);
    }

    public override void Update()
    {
        base.Update();

        if (Kaqing.GetPlayerManager().IsDeadState())
            return;

        //ElementalSkillHitPos_Aim();

        if (Timer >= MaxTimer)
        {
            GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
            return;
        }
        Timer += Time.deltaTime;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        if (Kaqing.GetPlayerManager().IsDeadState())
            return;

        ElementalSkillHitPos_Aim();

    }
    public override void Exit()
    {
        Kaqing.GetKaqingAimController().DestroyTargetOrb();
    }
}
