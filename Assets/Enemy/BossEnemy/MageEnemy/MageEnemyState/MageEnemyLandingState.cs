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
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ;
        StartAnimation("isLanding");
    }

    public override void Exit()
    {
        base.Exit();
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().useGravity = true;
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        StopAnimation("isLanding");
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }
}
