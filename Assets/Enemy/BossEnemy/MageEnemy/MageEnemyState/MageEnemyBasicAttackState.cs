using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyBasicAttackState : MageEnemyAttackState
{
    public MageEnemyBasicAttackState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        StartAnimation("GroundAttack" + GetMageEnemyStateMachine().MageEnemyData.CurrentAttackIndex);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("GroundAttack" + GetMageEnemyStateMachine().MageEnemyData.CurrentAttackIndex);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        UpdateAttackState("GroundAttack" + GetMageEnemyStateMachine().MageEnemyData.CurrentAttackIndex);
    }
}
