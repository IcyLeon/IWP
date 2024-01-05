using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyState : IState
{
    private MageEnemyStateMachine MageEnemyStateMachine;
    private Rigidbody rb;
    public MageEnemyStateMachine GetMageEnemyStateMachine()
    {
        return MageEnemyStateMachine;
    }

    public virtual void Enter()
    {
        rb = GetMageEnemyStateMachine().GetMageEnemy().GetRB();
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

    protected void UpdateTargetRotation()
    {
        if (GetMageEnemyStateMachine().MageEnemyData.CurrentTargetRotation != GetMageEnemyStateMachine().MageEnemyData.Target_Rotation)
        {
            GetMageEnemyStateMachine().MageEnemyData.CurrentTargetRotation = GetMageEnemyStateMachine().MageEnemyData.Target_Rotation;
            GetMageEnemyStateMachine().MageEnemyData.dampedTargetRotationPassedTime = 0f;
        }
        RotateTowardsTargetRotation();

    }
    private void RotateTowardsTargetRotation()
    {
        if (rb == null)
            return;

        float currentYAngle = rb.rotation.eulerAngles.y;

        if (currentYAngle == GetMageEnemyStateMachine().MageEnemyData.CurrentTargetRotation.eulerAngles.y)
        {
            GetMageEnemyStateMachine().MageEnemyData.dampedTargetRotationPassedTime = 0;
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, GetMageEnemyStateMachine().MageEnemyData.CurrentTargetRotation.eulerAngles.y, ref GetMageEnemyStateMachine().MageEnemyData.dampedTargetRotationCurrentVelocity, GetMageEnemyStateMachine().MageEnemyData.timeToReachTargetRotation - GetMageEnemyStateMachine().MageEnemyData.dampedTargetRotationPassedTime);
        GetMageEnemyStateMachine().MageEnemyData.dampedTargetRotationPassedTime += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        rb.MoveRotation(targetRotation);
    }
    public void UpdateTargetRotation_Instant(Quaternion quaternion)
    {
        SetTargetRotation(quaternion);

        GetMageEnemyStateMachine().MageEnemyData.CurrentTargetRotation = GetMageEnemyStateMachine().MageEnemyData.Target_Rotation;

        Quaternion targetRotation = Quaternion.Euler(0f, GetMageEnemyStateMachine().MageEnemyData.CurrentTargetRotation.eulerAngles.y, 0f);
        rb.MoveRotation(targetRotation);

    }
    protected void SetTargetRotation(Quaternion quaternion)
    {
        GetMageEnemyStateMachine().MageEnemyData.Target_Rotation = quaternion;
    }

    protected Vector3 GetTargetRotationDirection(float targetRotationAngle)
    {
        return Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;
    }


    public virtual void Update()
    {
        if (GetMageEnemyStateMachine().GetMageEnemy().IsDead() && !isDeadState())
        {
            GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyDeadState);
            return;
        }

        if (GetMageEnemyStateMachine().MageEnemyData.ShieldStatus && GetMageEnemyStateMachine().GetMageEnemy().GetCurrentElementalShield() <= 0)
        {
            GetMageEnemyStateMachine().ChangeState(GetMageEnemyStateMachine().MageEnemyStunState);
            return;
        }
    }

    private bool isDeadState()
    {
        return GetMageEnemyStateMachine().GetCurrentState() is MageEnemyDeadState;
    }
    protected bool isShieldState()
    {
        return GetMageEnemyStateMachine().GetCurrentState() is MageEnemyShieldState;
    }
    protected bool CanPerformAction()
    {
        return GetMageEnemyStateMachine().GetCurrentState() is MageEnemyIdleState || GetMageEnemyStateMachine().GetCurrentState() is MageEnemyChaseState;
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
