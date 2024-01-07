using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotShootState : PunchingBotAttackState
{
    public PunchingBotShootState(PunchingBotStateMachine p) : base(p)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isShooting");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isShooting");
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
