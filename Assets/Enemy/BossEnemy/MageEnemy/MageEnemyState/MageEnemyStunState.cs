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
        GetMageEnemyStateMachine().MageEnemyData.CurrentStunElasped = GetMageEnemyStateMachine().MageEnemyData.stunDuration;
        GetMageEnemyStateMachine().MageEnemyData.ShieldStatus = false;
        StartAnimation("isStun");
    }

    public override void Exit()
    {
        base.Exit();
        GetMageEnemyStateMachine().MageEnemyData.ShieldCurrentElasped = GetMageEnemyStateMachine().MageEnemyData.ShieldCooldown;
        StopAnimation("isStun");
        ResetStunState();
    }

    private void ResetStunState()
    {
        GetMageEnemyStateMachine().MageEnemyData.CurrentStunElasped = GetMageEnemyStateMachine().MageEnemyData.stunDuration;
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
