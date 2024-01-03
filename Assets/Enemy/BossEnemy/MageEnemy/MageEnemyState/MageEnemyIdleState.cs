using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyIdleState : MageEnemyGroundState
{
    private float WaitForAction = 0.75f;
    private float WaitForActionElapsed = 0f; 

    public MageEnemyIdleState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        WaitForActionElapsed = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        WaitForActionElapsed = 0f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateTargetRotation();
    }

    private bool isCloseToPlayer()
    {
        return GetMageEnemyStateMachine().GetMageEnemy().HasReachedTargetLocation(GetMageEnemyStateMachine().GetMageEnemy().GetPlayerLocation());
    }

    public override void Update()
    {
        base.Update();

        Vector3 targetdir = GetMageEnemyStateMachine().GetMageEnemy().GetTargetDirection();
        targetdir.y = 0f;
        SetTargetRotation(Quaternion.LookRotation(targetdir));

        if (WaitForActionElapsed > WaitForAction)
        {
            if (!isCloseToPlayer())
            {
                GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyChaseState);
            }
            return;
        }
        else
        {
            if (isCloseToPlayer())
            {
                UpdateBasicAttacks();
                return;
            }
        }

        WaitForActionElapsed += Time.deltaTime;
    }
}
