using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotIdle : PunchingBotState
{
    private float WaitForAction = 0.08f;
    private float WaitForActionElapsed = 0f;

    public PunchingBotIdle(PunchingBotStateMachine p) : base(p)
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
        return GetPunchingBotStateMachine().GetPunchingBot().HasReachedTargetLocation(GetPunchingBotStateMachine().GetPunchingBot().GetPlayerLocation());
    }

    public override void Update()
    {
        base.Update();

        Vector3 targetdir = GetPunchingBotStateMachine().GetPunchingBot().GetTargetDirection();
        targetdir.y = 0f;
        if (targetdir != default(Vector3))
            SetTargetRotation(Quaternion.LookRotation(targetdir));

        if (WaitForActionElapsed > WaitForAction)
        {
            GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotChaseState);
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

    private void UpdateBasicAttacks()
    {
        if (GetPunchingBotStateMachine().GetCurrentState() is MageEnemyAttackState)
            return;

        if (!CanPerformAction())
            return;

        if (Time.time - GetPunchingBotStateMachine().PunchingBotData.AttackCurrentElasped > GetPunchingBotStateMachine().PunchingBotData.AttackInterval)
        {
            //int randomAttack = Random.Range(0, 3);
            //switch (randomAttack)
            //{
            //    case 0:
            //        GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotPunchState);
            //        break;
            //    case 1:
            //        GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotShootState);
            //        break;
            //}
            GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotPunchState);
            GetPunchingBotStateMachine().PunchingBotData.AttackInterval = GetPunchingBotStateMachine().PunchingBotData.GetRandomAttackInterval();
            GetPunchingBotStateMachine().PunchingBotData.AttackCurrentElasped = Time.time;
            return;
        }
    }
}
