using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KaqingBurstState : PlayerElementalBurstState
{
    public KaqingState GetKaqingState()
    {
        return (KaqingState)GetPlayerCharacterState();
    }

    private enum ElementalBurst
    {
        First_Phase,
        Last_Hit,
    }
    private ElementalBurst elementalBurst;
    private int TotalHits = 10;
    private float TimeInBetweenHits;
    private float LastHitTimer;
    private float HitElapsed;
    private int CurrentHits;
    private bool StartBurst;
    private Vector3 LastPosition;

    public KaqingBurstState(PlayerCharacterState pcs) : base(pcs)
    {
        TimeInBetweenHits = 2.5f / (TotalHits * 2f);
    }

    public override void Enter()
    {
        base.Enter();
        StartBurst = false;
        GetKaqingState().GetKaqing().SpawnEffects();
        elementalBurst = ElementalBurst.First_Phase;
        CurrentHits = 0;
        HitElapsed = Time.time + TimeInBetweenHits;
        LastPosition = GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerOffsetPosition().position;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isBurst");
        HitElapsed = 0;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UpdateOffline()
    {
        UpdateBurstState();
    }

    private void BurstAreaDamage(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, GetKaqingState().KaqingData.UltiRange, LayerMask.GetMask("Entity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                if (!damage.IsDead())
                {
                    GetKaqingState().GetKaqing().SpawnHitEffect(damage);
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
                        GetKaqingState().GetKaqing().GetModel().SetActive(true);
                        GetKaqingState().GetKaqing().SetBurstActive(false);
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
                    if (Time.time - LastHitTimer > 0.8f)
                    {
                        BurstAreaDamage(LastPosition);
                        CurrentHits++;

                        GetKaqingState().ChangeState(GetKaqingState().kaqingIdleState);
                    }
                    break;
            }
        }
    }

    public override void OnAnimationTransition()
    {
        GetKaqingState().GetKaqing().GetModel().SetActive(false);
        GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerController().GetCameraManager().Recentering();
        StartBurst = true;
    }
}
