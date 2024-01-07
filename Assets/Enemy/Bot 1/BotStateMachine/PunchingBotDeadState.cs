using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotDeadState : PunchingBotState
{
    public PunchingBotDeadState(PunchingBotStateMachine p) : base(p)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isDead");
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isDead");
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
