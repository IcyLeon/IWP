using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyAttackState : MageEnemyGroundState
{
    public MageEnemyAttackState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
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

    protected void UpdateAttackState(string name)
    {
        GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;

        if (!GetMageEnemyStateMachine().GetMageEnemy().GetAnimator().GetCurrentAnimatorStateInfo(0).IsName(name))
            return;

        if (GetMageEnemyStateMachine().GetMageEnemy().GetAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            OnIdleState();
            return;
        }

    }

    protected void OnIdleState()
    {
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }
}
