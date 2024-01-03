using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MageEnemyChaseState : MageEnemyGroundState
{
    public MageEnemyChaseState(MageEnemyStateMachine MageEnemyStateMachine) : base(MageEnemyStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetMageEnemyStateMachine().GetMageEnemy().EnableAgent();

    }

    public override void Exit()
    {
        base.Exit();
        GetMageEnemyStateMachine().GetMageEnemy().DisableAgent();
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

        if (GetMageEnemyStateMachine().GetMageEnemy().GetNavMeshAgent().enabled)
        {
            if (NavMesh.SamplePosition(GetMageEnemyStateMachine().GetMageEnemy().GetNavMeshAgent().transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
            {
                GetMageEnemyStateMachine().GetMageEnemy().GetNavMeshAgent().SetDestination(GetMageEnemyStateMachine().GetMageEnemy().GetPlayerLocation());
            }
        }

        if (GetMageEnemyStateMachine().GetMageEnemy().HasReachedTargetLocation(GetMageEnemyStateMachine().GetMageEnemy().GetPlayerLocation()))
        {
            GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyIdleState);
            return;
        }

    }
}
