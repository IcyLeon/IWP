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

        Vector3 targetdir = GetMageEnemyStateMachine().GetMageEnemy().GetTargetDirection();
        targetdir.y = 0f;
        UpdateTargetRotation_Instant(Quaternion.LookRotation(targetdir));
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
        if (!GetMageEnemyStateMachine().GetMageEnemy().GetAnimator().GetBool(name))
            return;

        GetMageEnemyStateMachine().MageEnemyData.AttackCurrentElasped = Time.time;

        if (GetMageEnemyStateMachine().GetMageEnemy().GetAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            OnIdleState();
            return;
        }

    }

    protected void OnIdleState()
    {
        if (GetMageEnemyStateMachine().GetCurrentState() is not MageEnemyAirborneState)
            GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }
}
