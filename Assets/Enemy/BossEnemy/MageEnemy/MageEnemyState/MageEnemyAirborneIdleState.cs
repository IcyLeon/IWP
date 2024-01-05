using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyAirborneIdleState : MageEnemyAirborneState
{
    public MageEnemyAirborneIdleState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetMageEnemyStateMachine().MageEnemyData.FireBallSpawnElapsed = GetMageEnemyStateMachine().MageEnemyData.FireBallSpawnDuration;
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
}
