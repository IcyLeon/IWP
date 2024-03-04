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
        GetBowCharactersState().GetBowCharacters().GetPlayerManager().GetPlayerCanvasUI().ShowCrossHair(true);
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

    private Vector3 GetFirstTargetHits(float length)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit[] raycastsHitAll = GetBowCharactersState().GetBowCharacters().GetRayPositionAll3D(ray.origin, ray.direction, length);
        List<RaycastHit> hits = new List<RaycastHit>(raycastsHitAll);

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            RaycastHit hit = hits[i];
            if (Vector3.Distance(hit.point, ray.origin) < Vector3.Distance(ray.origin, GetBowCharactersState().GetBowCharacters().GetEmitterPivot().position))
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
        GetBowCharactersState().GetBowCharacters().GetAim().SetAimTargetPosition(GetFirstTargetHits(25f));
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

        BowData BowData = GetBowCharactersState().GetBowData();
        BowData.Direction = GetBowCharactersState().GetBowCharacters().GetAim().GetAimDirection(GetBowCharactersState().GetBowCharacters().GetEmitterPivot().position);

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
        GetBowCharactersState().GetBowData().isChargedFinish = false;

        GetBowCharactersState().GetBowCharacters().GetPlayerManager().GetPlayerCanvasUI().ShowCrossHair(false);
        GetBowCharactersState().GetBowCharacters().DestroyChargeUpEmitter();
    }
}
