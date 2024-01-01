using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyState : IState
{
    private MageEnemyStateMachine MageEnemyStateMachine;

    public MageEnemyStateMachine GetMageEnemyStateMachine()
    {
        return MageEnemyStateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnAnimationTransition()
    {
    }

    public virtual void Update()
    {

    }

    protected void StartAnimation(string animationString)
    {
        Animator animator = GetMageEnemyStateMachine().GetMageEnemy().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, true);
    }

    protected void StopAnimation(string animationString)
    {
        Animator animator = GetMageEnemyStateMachine().GetMageEnemy().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, false);
    }

    protected void OnStunChanged()
    {
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyStunState);
    }

    protected void OnShieldChanged()
    {
        GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyShieldState);
    }


    public MageEnemyState(MageEnemyStateMachine MageEnemyStateMachine)
    {
        this.MageEnemyStateMachine = MageEnemyStateMachine;
    }
}
