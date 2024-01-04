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
        TransitToIdleState();
    }

    private void TransitToIdleState() // prevent getting stuck at that animation
    {
        if (!GetMageEnemyStateMachine().GetMageEnemy().GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Flame"))
            return;

        if (GetMageEnemyStateMachine().GetMageEnemy().GetAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            OnIdleState();
            return;
        }
    }
}
