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
        GetBowCharactersState().GetBowData().isChargedFinish = false;
    }

    public override void Update()
    {
        base.Update();

        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

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
        LaunchChargedAttack();

        if (!AimHold)
        {
            DelayToIdleElasped = Time.time;
            StartDelay = true;
        }
    }

    private void LaunchChargedAttack()
    {
        if (Time.timeScale == 0)
            return;

        BowData BowData = GetBowCharactersState().GetBowData();
        if (Time.time - BowData.LastClickedTime > BowData.ChargedAttackRate)
        {
            BowData.ShootElemental = BowData.CurrentElemental;
            GetBowCharactersState().GetPlayerCharacters().GetAnimator().SetBool("Attack1", true);
            BowData.LastClickedTime = Time.time;
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


        if (GetBowCharactersState().GetBowData().ChargeElapsed < GetBowCharactersState().GetBowData().ChargedMaxElapsed)
        {
            GetBowCharactersState().GetBowData().ChargeElapsed += Time.deltaTime;
            GetBowCharactersState().GetBowData().CurrentElemental = Elemental.NONE;
        }
        else
        {
            if (!GetBowCharactersState().GetBowData().isChargedFinish)
            {
                GetBowCharactersState().GetBowCharacters().SpawnChargeUpFinish();
                GetBowCharactersState().GetBowData().isChargedFinish = true;
            }
            GetBowCharactersState().GetBowData().CurrentElemental = GetPlayerCharacterState().GetPlayerCharacters().GetCharacterData().GetPlayerCharacterSO().Elemental;
        }
    }

    public override void Exit()
    {
        base.Exit();
        ResetCharge();
        GetBowCharactersState().GetBowData().isChargedFinish = false;

        GetBowCharactersState().GetBowCharacters().DestroyCrossHair();
        GetBowCharactersState().GetBowCharacters().DestroyChargeUpEmitter();
    }
}
