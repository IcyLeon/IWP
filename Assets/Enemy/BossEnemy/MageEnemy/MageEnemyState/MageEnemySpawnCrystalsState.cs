using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemySpawnCrystalsState : MageEnemyState
{
    public MageEnemySpawnCrystalsState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isSpawnOrb");
        GetMageEnemyStateMachine().MageEnemyData.CrystalsCoreSpawnElasped = GetMageEnemyStateMachine().MageEnemyData.GetRandomSpawnCrystalsInterval();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isSpawnOrb");
        AssetManager.GetInstance().OpenMessageNotification("The mage is spawning energy cores, destroy them quickly");
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
