using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyDeadState : MageEnemyState
{
    public MageEnemyDeadState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().useGravity = true;
        GetMageEnemyStateMachine().GetMageEnemy().GetRB().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ;
        GetMageEnemyStateMachine().GetMageEnemy().DisableAgent();
        StartAnimation("isDead");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isDead");
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
