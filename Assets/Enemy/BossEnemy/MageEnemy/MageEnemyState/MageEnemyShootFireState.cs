using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyShootFireState : MageEnemyGroundState
{

    public MageEnemyShootFireState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isShooting");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isShooting");
        GetMageEnemyStateMachine().GetMageEnemy().TurnOFFireBreathingCollider();
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

    private void OnIdleState()
    {
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }
}
