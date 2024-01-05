using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyAirborneFireballAttackState : MageEnemyAirborneAttackState
{
    public MageEnemyAirborneFireballAttackState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isSpawningProjectiles");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Vector3 targetdir = GetMageEnemyStateMachine().GetMageEnemy().GetTargetDirection();
        targetdir.y = 0f;
        if (targetdir != default(Vector3))
            SetTargetRotation(Quaternion.LookRotation(targetdir));



    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateTargetRotation();
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        StopAnimation("isSpawningProjectiles");
    }
}
