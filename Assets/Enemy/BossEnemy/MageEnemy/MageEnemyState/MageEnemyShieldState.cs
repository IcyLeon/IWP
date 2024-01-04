using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyShieldState : MageEnemyState
{
    public MageEnemyShieldState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isShield");

        GetMageEnemyStateMachine().MageEnemyData.CrystalsCoreSpawnElasped = GetMageEnemyStateMachine().MageEnemyData.CrystalsCoreSpawnDuration;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isShield");
        AssetManager.GetInstance().OpenMessageNotification("Use elemental damage for quicker shield weakening");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetMageEnemyStateMachine().MageEnemyData.ShieldStatus = true;
        GetMageEnemyStateMachine().GetMageEnemy().SetCurrentElementalShield(GetMageEnemyStateMachine().GetMageEnemy().GetElementalShield());
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
    }

    public override void Update()
    {
        //base.Update();
    }
}
