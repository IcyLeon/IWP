using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyShootFireState : MageEnemyAttackState
{

    public MageEnemyShootFireState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isBreathingFire");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isBreathingFire");
        GetMageEnemyStateMachine().GetMageEnemy().TurnOFFireBreathingCollider();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        UpdateAttackState("isBreathingFire");
    }
}
