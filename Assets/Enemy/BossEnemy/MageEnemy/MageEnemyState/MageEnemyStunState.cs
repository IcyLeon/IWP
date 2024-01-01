using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyStunState : MageEnemyState
{
    public MageEnemyStunState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetMageEnemyStateMachine().GetMageEnemy().GetCapsuleCollider().height = GetMageEnemyStateMachine().MageEnemyData.TargetColliderHeight;
        GetMageEnemyStateMachine().MageEnemyData.CurrentStunElasped = GetMageEnemyStateMachine().MageEnemyData.stunDuration;
        GetMageEnemyStateMachine().MageEnemyData.ShieldStatus = false;
        StartAnimation("isStun");
    }

    public override void Exit()
    {
        base.Exit();
        GetMageEnemyStateMachine().GetMageEnemy().GetCapsuleCollider().height = GetMageEnemyStateMachine().GetMageEnemy().GetOriginalColliderHeight();
        StopAnimation("isStun");
        ResetStunState();
    }

    private void ResetStunState()
    {
        GetMageEnemyStateMachine().MageEnemyData.CurrentStunElasped = 0f;
    }

    public override void FixedUpdate()
    {
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
    }

    public override void Update()
    {
        //base.Update();

        if (GetMageEnemyStateMachine().MageEnemyData.CurrentStunElasped <= 0)
        {
            GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
            return;
        }
        GetMageEnemyStateMachine().MageEnemyData.CurrentStunElasped -= Time.deltaTime;
    }
}
