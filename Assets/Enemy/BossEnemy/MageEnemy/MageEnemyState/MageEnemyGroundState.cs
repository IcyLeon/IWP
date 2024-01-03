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

        if (!isShieldState())
        {
            if (CanPerformAction())
            {
                if (!GetMageEnemyStateMachine().MageEnemyData.ShieldStatus)
                {
                    if (GetMageEnemyStateMachine().MageEnemyData.ShieldCurrentElasped <= 0f)
                    {
                        OnShieldChanged();
                        return;
                    }
                    GetMageEnemyStateMachine().MageEnemyData.ShieldCurrentElasped -= Time.deltaTime;
                }
            }
        }
    }

    protected void UpdateBasicAttacks()
    {
        if (GetMageEnemyStateMachine().GetCurrentState() is MageEnemyAttackState)
            return;

        if (!CanPerformAction())
            return;

        if (Time.time - GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped > GetMageEnemyStateMachine().MageEnemyData.AttackInterval)
        {
            GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyChargeAttackState);
            GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;
            return;
        }
    }
}
