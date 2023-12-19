using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAimState : BowControlState
{
    public BowAimState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetBowCharactersState().BowData.isChargedFinish = false;
        StartAnimation("isAiming");
    }

    public override void Update()
    {
        base.Update();

        GetBowCharactersState().GetBowCharacters().InitHitPos_Aim();
        UpdateBowAim();
    }

    public override void ChargeTrigger()
    {
        GetBowCharactersState().GetBowCharacters().LaunchBasicAttack();
        GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);

        if (!Input.GetMouseButtonDown(1))
            GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0.5f);

    }

    private void UpdateBowAim()
    {
        GetPlayerCharacterState().GetPlayerCharacters().UpdateCameraAim();
        GetBowCharactersState().GetBowCharacters().SpawnChargeEmitter();

        if (Input.GetMouseButtonUp(1))
        {
            GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
            GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0f);
            return;
        }


        if (GetBowCharactersState().BowData.ChargeElapsed < GetBowCharactersState().BowData.ChargedMaxElapsed)
        {
            GetBowCharactersState().BowData.ChargeElapsed += Time.deltaTime;
        }
        else
        {
            if (!GetBowCharactersState().BowData.isChargedFinish)
            {
                GetBowCharactersState().GetBowCharacters().SpawnChargeUpFinish();
                GetBowCharactersState().BowData.isChargedFinish = true;
            }
            GetBowCharactersState().BowData.CurrentElemental = GetPlayerCharacterState().GetPlayerCharacters().GetCharacterData().GetPlayerCharacterSO().Elemental;
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isAiming");
        ResetCharge();
        GetBowCharactersState().BowData.isChargedFinish = false;

        GetBowCharactersState().GetBowCharacters().DestroyCrossHair();
        GetBowCharactersState().GetBowCharacters().DestroyChargeUpEmitter();
    }
}
