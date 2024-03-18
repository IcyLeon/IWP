using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotState : IState
{
    private PunchingBotStateMachine PunchingBotStateMachine;
    private Rigidbody rb;
    public PunchingBotState(PunchingBotStateMachine p)
    {
        PunchingBotStateMachine = p;
    }
    public PunchingBotStateMachine GetPunchingBotStateMachine()
    {
        return PunchingBotStateMachine;
    }

    public virtual void Enter()
    {
        rb = GetPunchingBotStateMachine().GetPunchingBot().GetRB();
    }

    public virtual void Exit()
    {
    }

    public virtual void FixedUpdate()
    {
        RotateTowardsTargetRotation();
    }
    public virtual void LateUpdate()
    {
    }
    public virtual void OnAnimationTransition()
    {
    }


    private void RotateTowardsTargetRotation()
    {
        if (rb == null)
            return;

        float currentYAngle = rb.rotation.eulerAngles.y;

        if (currentYAngle == GetPunchingBotStateMachine().PunchingBotData.CurrentTargetRotation.eulerAngles.y)
        {
            GetPunchingBotStateMachine().PunchingBotData.dampedTargetRotationPassedTime = 0;
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, GetPunchingBotStateMachine().PunchingBotData.CurrentTargetRotation.eulerAngles.y, ref GetPunchingBotStateMachine().PunchingBotData.dampedTargetRotationCurrentVelocity, GetPunchingBotStateMachine().PunchingBotData.timeToReachTargetRotation - GetPunchingBotStateMachine().PunchingBotData.dampedTargetRotationPassedTime);
        GetPunchingBotStateMachine().PunchingBotData.dampedTargetRotationPassedTime += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        rb.MoveRotation(targetRotation);
    }
    public void UpdateTargetRotation_Instant(Quaternion quaternion)
    {
        SetTargetRotation(quaternion);
        Quaternion targetRotation = Quaternion.Euler(0f, GetPunchingBotStateMachine().PunchingBotData.CurrentTargetRotation.eulerAngles.y, 0f);
        rb.MoveRotation(targetRotation);

    }
    protected void SetTargetRotation(Quaternion quaternion)
    {
        GetPunchingBotStateMachine().PunchingBotData.CurrentTargetRotation = quaternion;
        GetPunchingBotStateMachine().PunchingBotData.dampedTargetRotationPassedTime = 0f;
    }

    protected Vector3 GetTargetRotationDirection(float targetRotationAngle)
    {
        return Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;
    }


    public virtual void Update()
    {
        if (GetPunchingBotStateMachine().GetPunchingBot().IsDead() && !isDeadState())
        {
            GetPunchingBotStateMachine().ChangeState(GetPunchingBotStateMachine().PunchingBotDeadState);
            return;
        }
    }

    private bool isDeadState()
    {
        return GetPunchingBotStateMachine().GetCurrentState() is PunchingBotDeadState;
    }
    protected bool CanPerformAction()
    {
        return GetPunchingBotStateMachine().GetCurrentState() is PunchingBotIdle || GetPunchingBotStateMachine().GetCurrentState() is PunchingBotChaseState;
    }

    protected void StartAnimation(string animationString)
    {
        Animator animator = GetPunchingBotStateMachine().GetPunchingBot().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, true);
    }

    protected void StopAnimation(string animationString)
    {
        Animator animator = GetPunchingBotStateMachine().GetPunchingBot().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, false);
    }
}
