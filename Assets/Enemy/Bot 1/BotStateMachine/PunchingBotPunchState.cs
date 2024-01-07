using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotPunchState : PunchingBotAttackState
{
    public PunchingBotPunchState(PunchingBotStateMachine p) : base(p)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isPunching");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isPunching");
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotIdle);
    }

    public override void Update()
    {
        base.Update();
        UpdateAttackState();
    }
}
