using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotAttackState : PunchingBotState
{
    public PunchingBotAttackState(PunchingBotStateMachine p) : base(p)
    {
    }

    protected void UpdateAttackState()
    {
        GetPunchingBotStateMachine().PunchingBotData.AttackCurrentElasped = Time.time;

        if (GetPunchingBotStateMachine().GetPunchingBot().GetAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            OnIdleState();
            return;
        }

    }

    public override void Update()
    {
        base.Update();

        Vector3 targetdir = GetPunchingBotStateMachine().GetPunchingBot().GetTargetDirection();
        targetdir.y = 0f;
        if (targetdir != default(Vector3))
            SetTargetRotation(Quaternion.LookRotation(targetdir));


    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateTargetRotation();
    }

    protected void OnIdleState()
    {
        GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotIdle);
    }
}
