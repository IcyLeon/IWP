using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemySpawnMinionState : MageEnemyAttackState
{
    public MageEnemySpawnMinionState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isCalling");
        GetMageEnemyStateMachine().MageEnemyData.CallingEnemiesElasped = GetMageEnemyStateMachine().MageEnemyData.CallingEnemiesInterval;
        AssetManager.GetInstance().OpenMessageNotification("The mage is calling for reinforcement");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isCalling");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }

    public override void Update()
    {
        base.Update();
    }
}
