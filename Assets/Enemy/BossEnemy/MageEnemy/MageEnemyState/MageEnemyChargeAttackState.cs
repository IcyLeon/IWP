using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyChargeAttackState : MageEnemyAttackState
{
    public MageEnemyChargeAttackState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        StartAnimation("ChargeAttack");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("ChargeAttack");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        OnIdleState();
    }
    public override void Update()
    {
        base.Update();

        UpdateAttackState("ChargeAttack");
    }
}
