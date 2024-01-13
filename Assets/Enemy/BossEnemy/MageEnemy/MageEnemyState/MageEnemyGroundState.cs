using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyGroundState : MageEnemyState
{
    public MageEnemyGroundState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Update()
    {
        base.Update();

        if (CanPerformAction())
        {
            UpdateAttacksFarAway();

            if (!GetMageEnemyStateMachine().MageEnemyData.ShieldStatus)
            {
                if (GetMageEnemyStateMachine().MageEnemyData.ShieldCurrentElasped <= 0f)
                {
                    OnShieldChanged();
                    return;
                }
            }
            GetMageEnemyStateMachine().MageEnemyData.ShieldCurrentElasped -= Time.deltaTime;

            if (GetMageEnemyStateMachine().MageEnemyData.TakeOffElapsed <= 0)
            {
                GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyTakeOffState);
                return;
            }

            GetMageEnemyStateMachine().MageEnemyData.TakeOffElapsed -= Time.deltaTime;

        }
    }

    public override void Exit()
    {
        base.Exit();
        GetMageEnemyStateMachine().MageEnemyData.dampedTargetRotationPassedTime = 0f;
    }

    protected void UpdateAttacksFarAway()
    {
        if (isAirborne())
            return;


        if (GetMageEnemyStateMachine().MageEnemyData.CallingEnemiesElasped <= 0)
        {
            if (!GetMageEnemyStateMachine().MageEnemyData.ShieldStatus || !GetMageEnemyStateMachine().GetMageEnemy().CanSpawnReinforcement())
                return;

            if (GetMageEnemyStateMachine().GetCurrentState() is not MageEnemyAttackState)
            {
                GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemySpawnMinionState);
                return;
            }
        }
        GetMageEnemyStateMachine().MageEnemyData.CallingEnemiesElasped -= Time.deltaTime;


        if (GetMageEnemyStateMachine().GetMageEnemy().GetTotalMageNotDestroyed() == 0 && GetMageEnemyStateMachine().MageEnemyData.ShieldStatus
    && GetMageEnemyStateMachine().MageEnemyData.CrystalsCoreSpawnElasped > 0)
        {
            GetMageEnemyStateMachine().MageEnemyData.CrystalsCoreSpawnElasped -= Time.deltaTime;
        }

        if (GetMageEnemyStateMachine().MageEnemyData.CrystalsCoreSpawnElasped <= 0)
        {
            if (GetMageEnemyStateMachine().GetCurrentState() is not MageEnemyAttackState)
            {
                GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemySpawnCrystalsState);
                return;
            }
        }
    }

    protected void UpdateBasicAttacks()
    {
        if (GetMageEnemyStateMachine().GetCurrentState() is MageEnemyAttackState)
            return;

        if (!CanPerformAction() || isAirborne())
            return;

        if (Time.time - GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped > GetMageEnemyStateMachine().MageEnemyData.AttackInterval)
        {
            if (CanShootFlame())
            {
                GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyShootFireState);
                GetMageEnemyStateMachine().MageEnemyData.AttackInterval = GetMageEnemyStateMachine().MageEnemyData.GetRandomAttackInterval();
                GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;
                return;
            }

            int randomAttack = Random.Range(0, 3);
            switch(randomAttack)
            {
                case 0:
                    GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyBasicAttackState);
                    break;
                case 1:
                    GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyChargeAttackState);
                    break;
            }

            GetMageEnemyStateMachine().MageEnemyData.AttackInterval = GetMageEnemyStateMachine().MageEnemyData.GetRandomAttackInterval();
            GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;
            return;
        }
    }

    private bool CanShootFlame()
    {
        return AssetManager.isInProbabilityRange(0.35f);
    }
}
