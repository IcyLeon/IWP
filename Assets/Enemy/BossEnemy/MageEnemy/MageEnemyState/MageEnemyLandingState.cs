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
        //GetMageEnemyStateMachine().GetMageEnemy().GetRB().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ;
        StartAnimation("isLanding");
    }

    public override void Exit()
    {
        base.Exit();
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().constraints = RigidbodyConstraints.FreezeAll;
        StopAnimation("isLanding");
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().useGravity = true;
        GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }
}
