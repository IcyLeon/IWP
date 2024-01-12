using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyTakeOffState : MageEnemyAirborneState
{
    public MageEnemyTakeOffState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isTakingOff");
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        GetMageEnemyStateMachine().MageEnemyData.TakeOffElapsed = GetMageEnemyStateMachine().MageEnemyData.TakeOffCooldown;
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyAirborneIdleState);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isTakingOff");
    }
}
