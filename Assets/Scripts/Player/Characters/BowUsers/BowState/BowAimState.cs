using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAimState : BowControlState
{
    private bool AimHold;
    private float DelayToIdle = 0.25f, DelayToIdleElasped;
    private bool StartDelay;
    public static Action<bool> OnBowAimStatus;

    public BowAimState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetPlayerCharacterState().GetPlayerCharacters().ToggleAimCamera(true);
        OnBowAimStatus?.Invoke(true);
        StartDelay = false;
        DelayToIdleElasped = Time.time;
        GetBowCharactersState().GetBowData().isChargedFinish = false;
    }

    public override void Update()
    {
        base.Update();

        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

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

    public static Vector3 GetFirstTargetHits(Vector3 EmitterPosition, float length)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit[] raycastsHitAll = PlayerController.GetRayPositionAll3D(ray.origin, ray.direction, length);
        List<RaycastHit> hits = new List<RaycastHit>(raycastsHitAll);

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            RaycastHit hit = hits[i];
            if (Vector3.Distance(hit.point, ray.origin) < Vector3.Distance(ray.origin, EmitterPosition))
            {
                hits.Remove(hit);
            }
        }

        hits.Sort((hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        if (hits.Count > 0)
            return hits[0].point;

        return ray.origin + ray.direction * length;
    }

    private void UpdateBowAim()
    {
        GetBowCharactersState().GetBowCharacters().GetAimController().SetAimTargetPosition(GetFirstTargetHits(GetBowCharactersState().GetBowCharacters().GetEmitterPivot().position, 25f));
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
            return;
        }

        BowData BowData = GetBowCharactersState().GetBowData();
        BowData.Direction = GetBowCharactersState().GetBowCharacters().GetAimController().GetAimDirection(GetBowCharactersState().GetBowCharacters().GetEmitterPivot().position);

        if (BowData.ChargeElapsed < GetBowCharactersState().GetBowData().ChargedMaxElapsed)
        {
            BowData.ChargeElapsed += Time.deltaTime;
            BowData.CurrentElemental = Elemental.NONE;
        }
        else
        {
            if (!BowData.isChargedFinish)
            {
                GetBowCharactersState().GetBowCharacters().SpawnChargeUpFinish();
                BowData.isChargedFinish = true;
            }
            BowData.CurrentElemental = GetPlayerCharacterState().GetPlayerCharacters().GetCharacterData().GetPlayerCharacterSO().Elemental;
        }
    }

    public override void Exit()
    {
        base.Exit();
        ResetCharge();
        GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0f);
        GetBowCharactersState().GetBowData().isChargedFinish = false;
        OnBowAimStatus?.Invoke(false);
        GetBowCharactersState().GetBowCharacters().DestroyChargeUpEmitter();
    }
}
