using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PunchingBotChaseState : PunchingBotState
{
    public PunchingBotChaseState(PunchingBotStateMachine p) : base(p)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetPunchingBotStateMachine().GetPunchingBot().EnableAgent();

    }

    public override void Exit()
    {
        base.Exit();
        GetPunchingBotStateMachine().GetPunchingBot().DisableAgent();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
    }

    public override void Update()
    {
        base.Update();

        if (GetPunchingBotStateMachine().GetPunchingBot().GetNavMeshAgent().enabled)
        {
            if (NavMesh.SamplePosition(GetPunchingBotStateMachine().GetPunchingBot().GetNavMeshAgent().transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
            {
                GetPunchingBotStateMachine().GetPunchingBot().GetNavMeshAgent().SetDestination(GetPunchingBotStateMachine().GetPunchingBot().GetPlayerLocation());
            }
        }

        if (GetPunchingBotStateMachine().GetPunchingBot().HasReachedTargetLocation(GetPunchingBotStateMachine().GetPunchingBot().GetPlayerLocation()))
        {
            GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotIdle);
            return;
        }

    }
}
