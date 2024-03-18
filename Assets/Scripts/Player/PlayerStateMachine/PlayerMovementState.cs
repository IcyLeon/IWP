using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : IState
{
    private PlayerState PlayerState;
    private float WalkSpeed = 4.5f;
    private float Speed;
    private float DecelerateForce = 4.5f;
    protected PlayerCharacters playerCharacter;
    protected void SetSpeedModifier(float sp)
    {
        GetPlayerState().PlayerData.SpeedModifier = sp;
    }
    protected void SetSpeed(float sp)
    {
        Speed = sp;
    }

    protected float GetSpeed()
    {
        return Speed;
    }

    protected void ResetSpeed()
    {
        Speed = WalkSpeed;
    }

    protected float GetWalkSpeed()
    {
        return WalkSpeed;
    }

    public Vector3 GetInputDirection()
    {
        Vector2 InputValues = GetPlayerState().GetPlayerController().GetPlayerActions().Move.ReadValue<Vector2>();
        return InputValues;
    }

    protected virtual void SubscribeInput()
    {
        GetPlayerState().GetPlayerController().GetPlayerActions().Dash.started += OnDashAction;
    }
    protected virtual void UnsubscribeInput()
    {
        GetPlayerState().GetPlayerController().GetPlayerActions().Dash.started -= OnDashAction;
    }

    public PlayerMovementState(PlayerState playerState)
    {
        PlayerState = playerState;
        GetPlayerState().PlayerData.dampedTargetRotationPassedTime = 0;
    }

    protected bool IsBurstActive()
    {
        return GetPlayerState().GetPlayerManager().IsBurstActive();
    }

    public PlayerState GetPlayerState()
    {
        return PlayerState;
    }
    public virtual void Enter()
    {
        GetPlayerState().rb = GetPlayerState().GetPlayerManager().GetCharacterRB();
        SubscribeInput();
    }

    public virtual void Exit()
    {
        UnsubscribeInput();
    }

    protected void StartAnimation(string animationString)
    {
        Animator animator = GetPlayerState().GetPlayerManager().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, true);
    }

    protected void StopAnimation(string animationString)
    {
        Animator animator = GetPlayerState().GetPlayerManager().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, false);
    }

    public virtual void FixedUpdate()
    {
        UpdatePhysicsMovement();
    }

    protected bool IsAiming()
    {
        return GetPlayerState().GetPlayerController().GetCameraManager().GetAimCamera().gameObject.activeSelf;
    }

    protected bool IsAttacking()
    {
        return GetPlayerState().GetPlayerMovementState() is PlayerAttackState;
    }

    public virtual void Update()
    {
        UpdatePreviousPosition();
        playerCharacter = GetPlayerState().GetPlayerManager().GetCurrentCharacter();
        if (playerCharacter != null)
        {
            if (GetPlayerState().GetPlayerMovementState() is not PlayerDeadState)
            {
                if (playerCharacter.IsDead())
                {
                    GetPlayerState().ChangeState(GetPlayerState().playerDeadState);
                    return;
                }
            }
        }

        GatherInput();
    }
    public virtual void LateUpdate()
    {
    }
    protected void LimitFallVelocity()
    {
        float FallSpeedLimit = 35f;
        Vector3 velocity = GetVerticalVelocity();
        if (velocity.y >= -FallSpeedLimit)
        {
            return;
        }

        Vector3 limitVel = new Vector3(0f, -FallSpeedLimit - velocity.y, 0f);
        GetPlayerState().rb.AddForce(limitVel, ForceMode.VelocityChange);

    }
    public virtual float GetAnimationSpeed()
    {
        return 0;
    }

    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeSpeedAngles().Evaluate(angle);
        return slopeSpeedModifier;
    }

    protected void Float()
    {
        if (GetPlayerState().rb == null)
            return;

        if (GetPlayerState().GetPlayerController().GetResizeableCollider() == null || GetCapsuleCollider() == null)
            return;

        if (!GetPlayerState().rb.useGravity)
            return;

        Vector3 capsuleColliderCenterInWorldSpace = GetCapsuleCollider().bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if (Physics.Raycast(capsuleColliderCenterInWorldSpace, Vector3.down, out RaycastHit hit, GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().FloatRayDistance, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if (slopeSpeedModifier == 0f)
            {
                SetSpeedModifier(1f);
                return;
            }

            SetSpeedModifier(slopeSpeedModifier);

            GetPlayerState().PlayerData.HitDistance = hit.distance;
            float distanceToFloatingPoint = GetPlayerState().GetPlayerController().GetResizeableCollider().GetColliderCenterInLocalSpace().y * GetPlayerState().rb.transform.localScale.y - hit.distance;

            if (distanceToFloatingPoint == 0f)
            {
                return;
            }

            float amountToLift = distanceToFloatingPoint * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepReachForce - GetVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            GetPlayerState().rb.AddForce(liftForce, ForceMode.VelocityChange);
        }
        else
        {
            if (!IsTouchingTerrain())
                return;

            float distanceToFloatingPoint = GetPlayerState().GetPlayerController().GetResizeableCollider().GetColliderCenterInLocalSpace().y * GetPlayerState().rb.transform.localScale.y - GetPlayerState().PlayerData.HitDistance;
            float amountToLift = distanceToFloatingPoint * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepReachForce - GetVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            GetPlayerState().rb.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    public bool CheckIfisAboutToFall()
    {
        if (GetPlayerState().rb == null)
            return false;

        float horizontalDisCheck = 1f;
        float verticalDisCheck = 2f;
        Vector3 dir = GetPlayerState().rb.velocity;
        if (dir == Vector3.zero)
        {
            dir = GetPlayerState().GetPlayerManager().transform.forward;
            if (GetPlayerState().PlayerData.Direction != Vector3.zero)
                dir = GetPlayerState().PlayerData.Direction;
        }
        dir.y = 0f;
        dir.Normalize();

        if (Physics.Raycast(GetPlayerState().GetPlayerManager().GetPlayerOffsetPosition().position, dir, horizontalDisCheck, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        else
        {
            Vector3 hitpos = GetPlayerState().GetPlayerManager().GetPlayerOffsetPosition().position + dir * horizontalDisCheck;
            return !Physics.Raycast(hitpos, Vector3.down, verticalDisCheck, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }

    }


    private void UpdatePreviousPosition()
    {
        if (GetPlayerState().rb == null)
            return;

        if (this is not PlayerGroundState)
            return;

        if (CheckIfisAboutToFall())
            return;


        if (Physics.Raycast(GetPlayerState().GetPlayerManager().GetPlayerOffsetPosition().position, Vector3.down, out RaycastHit hit, float.MaxValue, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore))
        {
            GetPlayerState().PlayerData.PreviousPosition = hit.point;
        }
    }

    private void UpdatePhysicsMovement()
    {
        if (GetPlayerState().rb == null)
            return;

        if (GetPlayerState().GetPlayerManager().IsSkillCasting() || IsAttacking())
            return;

        if (!IsAiming())
        {
            HandleInputDirection();
            UpdateTargetRotation();
        }

        if (GetInputDirection() == Vector3.zero)
            return;

        if (GetPlayerState().GetPlayerController().GetLockMovement() == LockMovement.Enable)
            return;

        GetPlayerState().rb.AddForce((GetSpeed() * GetPlayerState().PlayerData.SpeedModifier * GetPlayerState().PlayerData.Direction) - GetHorizontalVelocity(), ForceMode.VelocityChange);
    }

    protected bool IsTouchingTerrain()
    {
        if (GetPlayerState().rb == null || GetCapsuleCollider() == null)
            return false;

        if (!GetPlayerState().rb.useGravity)
            return false;

        Vector3 capsuleColliderCenterInWorldSpace = GetCapsuleCollider().bounds.center;
        //Collider[] Colliders = Physics.OverlapSphere(capsuleColliderCenterInWorldSpace + Vector3.down * (GetPlayerState().GetPlayerController().GetResizeableCollider().GetDefaultColliderData_height() * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepHeightPercentage + GetCapsuleCollider().height / 2f - GetCapsuleCollider().radius / 2f), GetCapsuleCollider().radius / 1.5f, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore);
        return Physics.CheckSphere(capsuleColliderCenterInWorldSpace + Vector3.down * (GetPlayerState().GetPlayerController().GetResizeableCollider().GetDefaultColliderData_height() * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepHeightPercentage + GetCapsuleCollider().height / 2f - GetCapsuleCollider().radius / 2f), GetCapsuleCollider().radius, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision", "Player"), QueryTriggerInteraction.Ignore);
    }

    protected CapsuleCollider GetCapsuleCollider()
    {
        if (GetPlayerState().GetPlayerManager() == null)
            return null;

        if (GetPlayerState().GetPlayerManager().GetCurrentCharacter() == null)
            return null;

        return GetPlayerState().GetPlayerManager().GetCurrentCharacter().GetComponent<CapsuleCollider>();
    }

    protected void HandleInputDirection()
    {
        if (GetPlayerState().PlayerData.Direction == Vector3.zero)
            return;

        SetTargetRotation(Quaternion.LookRotation(GetPlayerState().PlayerData.Direction));
    }

    protected void SetTargetRotation(Quaternion quaternion)
    {
        GetPlayerState().PlayerData.CurrentTargetRotation = quaternion;
        GetPlayerState().PlayerData.dampedTargetRotationPassedTime = 0f;
    }

    protected Vector3 GetTargetRotationDirection(float targetRotationAngle)
    {
        return Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;
    }

    protected void UpdateTargetRotation()
    {
        if (GetPlayerState().rb == null)
            return;

        float currentYAngle = GetPlayerState().rb.rotation.eulerAngles.y;

        if (currentYAngle == GetPlayerState().PlayerData.CurrentTargetRotation.eulerAngles.y)
        {
            GetPlayerState().PlayerData.dampedTargetRotationPassedTime = 0;
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, GetPlayerState().PlayerData.CurrentTargetRotation.eulerAngles.y, ref GetPlayerState().PlayerData.dampedTargetRotationCurrentVelocity, PlayerData.timeToReachTargetRotation - GetPlayerState().PlayerData.dampedTargetRotationPassedTime);
        GetPlayerState().PlayerData.dampedTargetRotationPassedTime += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        GetPlayerState().rb.MoveRotation(targetRotation);
    }

    protected Vector3 GetHorizontalVelocity()
    {
        if (GetPlayerState().rb == null)
            return Vector3.zero;

        Vector3 vel = GetPlayerState().rb.velocity;
        vel.y = 0;
        return vel;
    }

    protected Vector3 GetVerticalVelocity()
    {
        if (GetPlayerState().rb == null)
            return Vector3.zero;

        return new Vector3(0f, GetPlayerState().rb.velocity.y, 0f);
    }


    protected void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetVerticalVelocity();
        GetPlayerState().rb.AddForce(DecelerateForce * -playerVerticalVelocity, ForceMode.Acceleration);
    }
    protected void DecelerateHorizontal()
    {
        Vector3 playerHorizontalVelocity = GetHorizontalVelocity();
        GetPlayerState().rb.AddForce(DecelerateForce * -playerHorizontalVelocity, ForceMode.Acceleration);
    }

    protected bool IsMovingUp(float minimumVelocity = 0f)
    {
        return GetVerticalVelocity().y > minimumVelocity;
    }

    protected bool IsMovingDown(float minimumVelocity = 0f)
    {
        return GetVerticalVelocity().y < -minimumVelocity;
    }

    protected bool IsMovingHorizontally(float minimumVelocity = 0f)
    {
        Vector2 horizontal = new Vector2(GetHorizontalVelocity().x, GetHorizontalVelocity().z);
        return horizontal.magnitude > minimumVelocity;
    }

    public void ResetVelocity()
    {
        if (GetPlayerState().rb == null)
            return;

        GetPlayerState().rb.velocity = Vector3.zero;
    }

    protected void ResetVerticalVelocity()
    {
        if (GetPlayerState().rb == null)
            return;

        Vector3 vel = GetPlayerState().rb.velocity;
        vel.y = 0;
        GetPlayerState().rb.velocity = vel;
    }

    private void GatherInput()
    {
        GetPlayerState().PlayerData.Direction = (Camera.main.transform.forward * GetInputDirection().y) + (Camera.main.transform.right * GetInputDirection().x);
        GetPlayerState().PlayerData.Direction.y = 0;
        GetPlayerState().PlayerData.Direction.Normalize();
    }

    private void OnDashAction(InputAction.CallbackContext CallbackContext)
    {
        if (!GetPlayerState().GetPlayerManager().GetStaminaManager().CanPerformDash() || !CanDash())
            return;

        if (GetPlayerState().GetPlayerManager().CanPerformAction())
            GetPlayerState().ChangeState(GetPlayerState().playerDashState);
    }

    private bool CanDash()
    {
        return GetPlayerState().GetPlayerController().GetPlayerActions().Dash.enabled;
    }

    public virtual void OnAnimationTransition()
    {

    }
}
