using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyAirborneState : MageEnemyState
{
    public MageEnemyAirborneState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().useGravity = false;
        GetMageEnemyStateMachine().GetMageEnemy().DisableAgent();
    }

    public override void Update()
    {
        base.Update();

        if (!isAttackState() && GetMageEnemyStateMachine().GetCurrentState() is not MageEnemyLandingState)
        {
            if (GetMageEnemyStateMachine().MageEnemyData.FireBallSpawnElapsed <= 0)
            {
                GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyAirborneFireballAttackState);
                return;
            }
            GetMageEnemyStateMachine().MageEnemyData.FireBallSpawnElapsed -= Time.deltaTime;
        }
    }

    protected bool isAttackState()
    {
        return GetMageEnemyStateMachine().GetCurrentState() is MageEnemyAirborneAttackState;
    } 
}
