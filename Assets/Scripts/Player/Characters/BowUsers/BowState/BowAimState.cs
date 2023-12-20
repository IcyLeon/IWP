using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAimState : BowControlState
{
    private bool AimHold;
    private float DelayToIdle = 0.25f, DelayToIdleElasped;
    private bool StartDelay;
    public BowAimState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartDelay = false;
        DelayToIdleElasped = Time.time;
        GetBowCharactersState().BowData.isChargedFinish = false;
        StartAnimation("isAiming");
    }

    public override void Update()
    {
        base.Update();
        GetBowCharactersState().GetBowCharacters().InitHitPos_Aim();
        UpdateBowAim();

        if (StartDelay)
        {
            if (Time.time - DelayToIdleElasped > DelayToIdle)
            {
                GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
                GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0);
                DelayToIdleElasped = Time.time;
            }
        }
    }

    public override void ChargeTrigger()
    {
        GetBowCharactersState().GetBowCharacters().LaunchBasicAttack();

        if (!AimHold)
        {
            DelayToIdleElasped = Time.time;
            StartDelay = true;
        }
    }

    public override void ChargeHold()
    {
        DelayToIdleElasped = Time.time;
        StartDelay = false;
    }

    private void UpdateBowAim()
    {
        GetPlayerCharacterState().GetPlayerCharacters().UpdateCameraAim();
        GetBowCharactersState().GetBowCharacters().SpawnChargeEmitter();

        AimHold = Input.GetMouseButton(1);

        if (AimHold)
        {
            DelayToIdleElasped = Time.time;
            StartDelay = false;
        }
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
