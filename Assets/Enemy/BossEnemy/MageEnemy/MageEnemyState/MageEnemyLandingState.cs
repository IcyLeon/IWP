using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyLandingState : MageEnemyAirborneState
{
    public MageEnemyLandingState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isLanding");
    }

    public override void Exit()
    {
        base.Exit();
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().useGravity = true;
        StopAnimation("isLanding");
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }
}
