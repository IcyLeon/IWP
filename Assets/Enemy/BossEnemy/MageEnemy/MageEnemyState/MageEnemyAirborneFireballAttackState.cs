using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyAirborneFireballAttackState : MageEnemyAirborneAttackState
{
    private float WaitTillTransitDuration = 2f, WaitTillTransitElapsed;
    private bool FireBallLaunched;
    public MageEnemyAirborneFireballAttackState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        FireBallLaunched = false;
        WaitTillTransitElapsed = WaitTillTransitDuration;
        GetMageEnemyStateMachine().MageEnemyData.FireBallSpawnElapsed = GetMageEnemyStateMachine().MageEnemyData.FireBallSpawnDuration;
        StartAnimation("isSpawningProjectiles");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Vector3 targetdir = GetMageEnemyStateMachine().GetMageEnemy().GetTargetDirection();
        targetdir.y = 0f;
        if (targetdir != default(Vector3))
            SetTargetRotation(Quaternion.LookRotation(targetdir));

        if (CanTransit() && FireBallLaunched)
        {
            if (WaitTillTransitElapsed <= 0)
            {
                GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyLandingState);
                return;
            }
            WaitTillTransitElapsed -= Time.deltaTime;
        }
    }

    private bool CanTransit()
    {
        return GetMageEnemyStateMachine().GetMageEnemy().GetTotalMageFireBall() == 0;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateTargetRotation();
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        StopAnimation("isSpawningProjectiles");
        FireBallLaunched = true;
    }
}
