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
        StartAnimation("isTakingOff");
        base.Enter();
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

    public override void Update()
    {
    }
}
