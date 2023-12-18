using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class PlayerMovementState : IState
{
    private PlayerState PlayerState;
    private float WalkSpeed;
    private float Speed;
    protected Rigidbody rb;
    private float HitDistance;
    private float DecelerateForce = 3.5f;

    protected void SetSpeedModifier(float sp)
    {
        GetPlayerState().PlayerData.SpeedModifier = sp;
    }
    protected void SetSpeed(float sp)
    {
        Speed = sp;
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
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        float VerticalInput = Input.GetAxisRaw("Vertical");

        Vector3 InputDirection = new Vector3(HorizontalInput, 0f, VerticalInput);
        InputDirection.Normalize();

        return InputDirection;
    }
    public PlayerMovementState(PlayerState playerState)
    {
        PlayerState = playerState;
        WalkSpeed = 5f;
        GetPlayerState().PlayerData.dampedTargetRotationPassedTime = 0;
    }

    protected bool IsBurstActive()
    {
        return GetPlayerState().GetPlayerController().GetPlayerManager().isBurstActive();
    }

    public PlayerState GetPlayerState()
    {
        return PlayerState;
    }
    public virtual void Enter()
    {
        rb = GetPlayerState().GetPlayerController().GetPlayerManager().GetCharacterRB();
    }

    public virtual void Exit()
    {
    }

    protected void StartAnimation(string animationString)
    {
        Animator animator = GetPlayerState().GetPlayerController().GetPlayerManager().GetAnimator();
        if (animator == null)
            return;

        animator.SetBool(animationString, true);
    }

    protected void StopAnimation(string animationString)
    {
        Animator animator = GetPlayerState().GetPlayerController().GetPlayerManager().GetAnimator();
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

    public virtual void Update()
    {
        rb = GetPlayerState().GetPlayerController().GetPlayerManager().GetCharacterRB();
        GatherInput();
        UpdateDash();
        UpdatePreviousPosition();

        PlayerCharacters playerCharacter = GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter();
        if (playerCharacter != null)
        {
            if (GetPlayerState().GetPlayerMovementState() is not PlayerDeadState)
            {
                if (playerCharacter.GetisAttacking()) {
                    GetPlayerState().ChangeState(GetPlayerState().playerAttackState);
                    return;
                }
                if (playerCharacter.IsDead())
                {
                    GetPlayerState().ChangeState(GetPlayerState().playerDeadState);
                    return;
                }
            }
        }
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
        rb.AddForce(limitVel, ForceMode.VelocityChange);

    }
    public virtual float GetAnimationSpeed()
    {
        return 0;
    }

    private void UpdateDash()
    {
        if (GetPlayerState().PlayerData.DashLimitReachedElasped > 0)
            GetPlayerState().PlayerData.DashLimitReachedElasped -= Time.deltaTime;

        GetPlayerState().PlayerData.DashLimitReachedElasped = Mathf.Clamp(GetPlayerState().PlayerData.DashLimitReachedElasped, 0f, GetPlayerState().PlayerData.DashLimitReachedCooldown);
    }
    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeSpeedAngles().Evaluate(angle);
        return slopeSpeedModifier;
    }

    protected void Float()
    {
        if (rb == null)
            return;

        if (GetPlayerState().GetPlayerController().GetResizeableCollider() == null || GetCapsuleCollider() == null)
            return;

        Vector3 capsuleColliderCenterInWorldSpace = GetCapsuleCollider().bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if (Physics.Raycast(capsuleColliderCenterInWorldSpace, Vector3.down, out RaycastHit hit, GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().FloatRayDistance))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if (slopeSpeedModifier == 0f)
            {
                SetSpeedModifier(1f);
                return;
            }

            SetSpeedModifier(slopeSpeedModifier);

            HitDistance = hit.distance;
            float distanceToFloatingPoint = GetPlayerState().GetPlayerController().GetResizeableCollider().GetColliderCenterInLocalSpace().y * rb.transform.localScale.y - hit.distance;

            if (distanceToFloatingPoint == 0f)
            {
                return;
            }

            float amountToLift = distanceToFloatingPoint * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepReachForce - GetVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            rb.AddForce(liftForce, ForceMode.VelocityChange);
        }
        else
        {
            if (!IsTouchingTerrain())
                return;

            float distanceToFloatingPoint = GetPlayerState().GetPlayerController().GetResizeableCollider().GetColliderCenterInLocalSpace().y * rb.transform.localScale.y - GetHitDistance();
            float amountToLift = distanceToFloatingPoint * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepReachForce - GetVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            rb.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    public bool CheckIfisAboutToFall()
    {
        if (rb == null)
            return false;

        float horizontalDisCheck = 1f;
        float verticalDisCheck = 1.5f;
        Vector3 dir = rb.velocity;
        if (dir == Vector3.zero)
            dir = GetPlayerState().GetPlayerController().transform.forward;
        dir.y = 0f;
        dir.Normalize();

        if (Physics.Raycast(GetPlayerState().GetPlayerController().GetPlayerManager().GetPlayerOffsetPosition().position, dir, horizontalDisCheck, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        else
        {
            Vector3 hitpos = GetPlayerState().GetPlayerController().GetPlayerManager().GetPlayerOffsetPosition().position + dir * horizontalDisCheck;
            return !Physics.Raycast(hitpos, Vector3.down, verticalDisCheck, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }

    }


    private void UpdatePreviousPosition()
    {
        if (rb == null)
            return;

        if (this is not PlayerGroundState)
            return;

        if (CheckIfisAboutToFall())
            return;


        if (Physics.Raycast(GetPlayerState().GetPlayerController().GetPlayerManager().GetPlayerOffsetPosition().position, Vector3.down, out RaycastHit hit, float.MaxValue, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            GetPlayerState().PlayerData.PreviousPosition = hit.point;
        }
    }

    public float GetHitDistance()
    {
        return HitDistance;
    }

    private void UpdatePhysicsMovement()
    {
        if (rb == null)
            return;

        if (this is PlayerDeadState || this is PlayerAttackState)
            return;

        if (this is not PlayerAimState)
        {
            UpdateInputTargetQuaternion();
            UpdateTargetRotation();
        }

        if (GetInputDirection() == Vector3.zero || this is PlayerDashState)
            return;

        if (GetPlayerState().GetPlayerController().GetLockMovement() == LockMovement.Enable || GetPlayerState().PlayerData.SpeedModifier == 0)
            return;

        rb.AddForce((GetPlayerState().PlayerData.Direction * Speed * GetPlayerState().PlayerData.SpeedModifier) - GetHorizontalVelocity(), ForceMode.VelocityChange);
    }

    protected bool IsTouchingTerrain()
    {
        if (rb == null || GetCapsuleCollider() == null)
            return false;


        int layerMask = Physics.DefaultRaycastLayers;
        Vector3 capsuleColliderCenterInWorldSpace = GetCapsuleCollider().bounds.center;
        bool isGrounded = Physics.CheckSphere(capsuleColliderCenterInWorldSpace + Vector3.down * (GetPlayerState().GetPlayerController().GetResizeableCollider().GetDefaultColliderData_height() * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepHeightPercentage + GetCapsuleCollider().height / 2f - GetCapsuleCollider().radius / 2f), GetCapsuleCollider().radius / 1.5f, layerMask, QueryTriggerInteraction.Ignore);
        return isGrounded;
    }

    protected CapsuleCollider GetCapsuleCollider()
    {
        if (GetPlayerState().GetPlayerController().GetPlayerManager() == null)
            return null;

        if (GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter() == null)
            return null;

        return GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter().GetComponent<CapsuleCollider>();
    }

    public void UpdateInputTargetQuaternion()
    {
        if (GetPlayerState().PlayerData.Direction == Vector3.zero)
            return;

        SetTargetRotation(Quaternion.LookRotation(GetPlayerState().PlayerData.Direction));
    }

    protected void UpdateTargetRotation()
    {
        if (GetPlayerState().PlayerData.CurrentTargetRotation != GetPlayerState().PlayerData.Target_Rotation)
        {
            GetPlayerState().PlayerData.CurrentTargetRotation = GetPlayerState().PlayerData.Target_Rotation;
            GetPlayerState().PlayerData.dampedTargetRotationPassedTime = 0f;
        }
        RotateTowardsTargetRotation();

    }

    public void UpdateTargetRotation_Instant(Quaternion quaternion)
    {
        SetTargetRotation(quaternion);

        GetPlayerState().PlayerData.CurrentTargetRotation = GetPlayerState().PlayerData.Target_Rotation;

        Quaternion targetRotation = Quaternion.Euler(0f, GetPlayerState().PlayerData.CurrentTargetRotation.eulerAngles.y, 0f);
        rb.MoveRotation(targetRotation);

    }

    protected void SetTargetRotation(Quaternion quaternion)
    {
        GetPlayerState().PlayerData.Target_Rotation = quaternion;
    }

    protected Vector3 GetTargetRotationDirection(float targetRotationAngle)
    {
        return Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;
    }

    private void RotateTowardsTargetRotation()
    {
        if (rb == null)
            return;

        float currentYAngle = rb.rotation.eulerAngles.y;

        if (currentYAngle == GetPlayerState().PlayerData.CurrentTargetRotation.eulerAngles.y)
        {
            GetPlayerState().PlayerData.dampedTargetRotationPassedTime = 0;
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, GetPlayerState().PlayerData.CurrentTargetRotation.eulerAngles.y, ref GetPlayerState().PlayerData.dampedTargetRotationCurrentVelocity, GetPlayerState().PlayerData.timeToReachTargetRotation - GetPlayerState().PlayerData.dampedTargetRotationPassedTime);
        GetPlayerState().PlayerData.dampedTargetRotationPassedTime += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        rb.MoveRotation(targetRotation);
    }

    protected Vector3 GetHorizontalVelocity()
    {
        if (rb == null)
            return Vector3.zero;

        Vector3 vel = rb.velocity;
        vel.y = 0;
        return vel;
    }

    protected Vector3 GetVerticalVelocity()
    {
        if (rb == null)
            return Vector3.zero;

        return new Vector3(0f, rb.velocity.y, 0f);
    }


    protected void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetVerticalVelocity();
        rb.AddForce(-playerVerticalVelocity * DecelerateForce, ForceMode.Acceleration);
    }
    protected void DecelerateHorizontal()
    {
        Vector3 playerHorizontalVelocity = GetHorizontalVelocity();
        rb.AddForce(-playerHorizontalVelocity * DecelerateForce, ForceMode.Acceleration);
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
        if (rb == null)
            return;

        rb.velocity = Vector3.zero;
    }

    protected void ResetVerticalVelocity()
    {
        if (rb == null)
            return;

        Vector3 vel = rb.velocity;
        vel.y = 0;
        rb.velocity = vel;
    }

    private void GatherInput()
    {
        GetPlayerState().PlayerData.Direction = (Camera.main.transform.forward * GetInputDirection().z) + (Camera.main.transform.right * GetInputDirection().x);
        GetPlayerState().PlayerData.Direction.y = 0;
        GetPlayerState().PlayerData.Direction.Normalize();
    }

    protected void OnDashInput()
    {
        //if (!GetPlayerState().GetPlayerController().GetPlayerManager().GetStaminaManager().CanPerformDash() || !CanDash() || IsAiming())
        //    return;

        if (!GetPlayerState().GetPlayerController().GetPlayerManager().GetStaminaManager().CanPerformDash() || !CanDash())
            return;

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && GetPlayerState().GetPlayerController().GetPlayerManager().CanAttack())
            GetPlayerState().ChangeState(GetPlayerState().playerDashState);
    }

    private bool CanDash()
    {
        return GetPlayerState().PlayerData.DashLimitReachedElasped <= 0;
    }

    public virtual void OnAnimationTransition()
    {

    }
}
