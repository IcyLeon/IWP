using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KaqingBurstState : SwordElementalBurstState
{
    private Kaqing Kaqing;

    protected KaqingState GetKaqingState()
    {
        return (KaqingState)GetPlayerCharacterState();
    }

    private enum ElementalBurst
    {
        First_Phase,
        Last_Hit,
    }
    private ElementalBurst elementalBurst;
    private int TotalHits = 12;
    private float TimeInBetweenHits;
    private float LastHitTimer;
    private float HitElapsed;
    private int CurrentHits;
    private bool StartBurst;
    private Vector3 LastPosition;

    public KaqingBurstState(PlayerCharacterState pcs) : base(pcs)
    {
        Kaqing = GetKaqingState().GetKaqing();
        TimeInBetweenHits = 2.5f / (TotalHits * 2f);
    }

    public override void Enter()
    {
        base.Enter();
        StartBurst = false;
        Kaqing.SpawnEffects();
        elementalBurst = ElementalBurst.First_Phase;
        CurrentHits = 0;
        HitElapsed = Time.time + TimeInBetweenHits;
        LastPosition = Kaqing.GetPlayerManager().GetPlayerOffsetPosition().position;

        Kaqing.GetPlayerManager().GetPlayerElementalSkillandBurstManager().SubscribeBurstState(Kaqing);
    }

    public override void Exit()
    {
        base.Exit();
        HitElapsed = 0;
    }

    public override void UpdateBurst()
    {
        UpdateBurstState();
    }

    private void BurstAreaDamage(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, Kaqing.GetUltiRange(), LayerMask.GetMask("Entity", "BossEntity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage damage = colliders[i].GetComponent<IDamage>();
            if (damage != null)
            {
                if (!damage.IsDead())
                {
                    Kaqing.SpawnHitEffect(damage);
                }
            }
        }
    }

    private void UpdateBurstState()
    {
        if (!StartBurst)
            return;

        if (CurrentHits < TotalHits)
        {
            switch (elementalBurst)
            {
                case ElementalBurst.First_Phase:
                    bool Visible = CurrentHits >= TotalHits / 2f;

                    if (Visible)
                    {
                        Kaqing.GetModel().SetActive(true);
                        Kaqing.SetBurstActive(false);
                        StopAnimation("isBurst");
                    }

                    if (CurrentHits >= TotalHits - 1)
                    {
                        elementalBurst = ElementalBurst.Last_Hit;
                        LastHitTimer = Time.time;
                        return;
                    }

                    if (Time.time - HitElapsed > TimeInBetweenHits)
                    {
                        BurstAreaDamage(LastPosition);
                        HitElapsed = Time.time;
                        CurrentHits++;
                    }
                    break;
                case ElementalBurst.Last_Hit:
                    Kaqing.DestroyUltiSlash();
                    if (Time.time - LastHitTimer > 0.8f)
                    {
                        BurstAreaDamage(LastPosition);
                        CurrentHits++;

                        GetKaqingState().ChangeState(GetKaqingState().swordIdleState);
                    }
                    break;
            }
        }
    }

    public override void OnAnimationTransition()
    {
        Kaqing.GetModel().SetActive(false);
        Kaqing.GetPlayerManager().GetPlayerController().GetCameraManager().Recentering();
        StartBurst = true;
        Kaqing.SpawnUltiSlash();
    }
}
