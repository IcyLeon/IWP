using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyIdleState : MageEnemyState
{
    public MageEnemyIdleState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
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

    public override void Update()
    {
        base.Update();


        if (GetMageEnemyStateMachine().MageEnemyData.ShieldStatus && GetMageEnemyStateMachine().GetMageEnemy().GetCurrentElementalShield() <= 0)
        {
            GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyStunState);
            return;
        }


        if (Input.GetKeyDown(KeyCode.K))
        {
            OnShieldChanged();
        }
    }
}
