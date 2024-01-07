using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotStateMachine : CharacterState
{
    public PunchingBotData PunchingBotData;
    public PunchingBotIdle PunchingBotIdle { get; }
    public PunchingBotPunchState PunchingBotPunchState { get; }
    public PunchingBotShootState PunchingBotShootState { get; }
    public PunchingBotDeadState PunchingBotDeadState { get; }
    public PunchingBotChaseState PunchingBotChaseState { get; }
    public PunchingBot GetPunchingBot()
    {
        return (PunchingBot)Characters;
    }
    public PunchingBotStateMachine(Characters Characters) : base(Characters)
    {
        PunchingBotData = new PunchingBotData();
        PunchingBotDeadState = new PunchingBotDeadState(this);
        PunchingBotPunchState = new PunchingBotPunchState(this);
        PunchingBotShootState = new PunchingBotShootState(this);
        PunchingBotIdle = new PunchingBotIdle(this);
        PunchingBotChaseState = new PunchingBotChaseState(this);

        ChangeState(PunchingBotIdle);
    }
}
