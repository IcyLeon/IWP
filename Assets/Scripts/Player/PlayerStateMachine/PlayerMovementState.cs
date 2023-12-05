using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementState : IState
{
    public enum PlayerStateEnum
    {
        IDLE,
        WALK,
        DASH,
        SPRINTING,
        STOPPING,
        JUMP,
        FALL,
        PLUNGE
    }

    private PlayerState PlayerState;

    private float Speed;
    protected Rigidbody rb;

    protected PlayerStateEnum playerStateEnum;

    public PlayerStateEnum GetPlayerStateEnum()
    {
        return playerStateEnum;
    }
    protected void SetSpeedModifier(float sp)
    {
        GetPlayerState().PlayerData.SpeedModifier = sp;
    }

    protected void ResetSpeedModifier()
    {
        GetPlayerState().PlayerData.SpeedModifier = 0;
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
        Speed = 4f;
        GetPlayerState().PlayerData.dampedTargetRotationPassedTime = 0;
    }

    protected bool IsBurstActive()
    {
        return GetPlayerState().GetPlayerController().isBurstActive();
    }

    public PlayerState GetPlayerState()
    {
        return PlayerState;
    }
    public virtual void Enter()
    {
        rb = GetPlayerState().GetPlayerController().GetCharacterRB();
    }

    public virtual void Exit()
    {
    }

    public float GetSpeed()
    {
        return Mathf.Pow(GetHorizontalVelocity().magnitude, 0.5f);
    }

    public virtual void FixedUpdate()
    {
        UpdatePhysicsMovement();
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }

    protected bool IsAiming()
    {
        return GetPlayerState().GetPlayerController().GetCameraManager().GetAimCamera().gameObject.activeSelf;
    }

    public virtual void Update()
    {
        rb = GetPlayerState().GetPlayerController().GetCharacterRB();
        GatherInput();
        UpdateDash();

        PlayerCharacters playerCharacter = GetPlayerState().GetPlayerController().GetCharacterManager().GetCurrentCharacter();
        if (playerCharacter != null)
        {
            if (playerCharacter.GetisAttacking())
            {
                GetPlayerState().ChangeState(GetPlayerState().playerAttackState);
            }
        }
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
                ResetSpeedModifier();
                return;
            }

            SetSpeedModifier(slopeSpeedModifier);

            float distanceToFloatingPoint = GetPlayerState().GetPlayerController().GetResizeableCollider().GetColliderCenterInLocalSpace().y * rb.transform.localScale.y - hit.distance;

            if (distanceToFloatingPoint == 0f)
            {
                return;
            }

            float amountToLift = distanceToFloatingPoint * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepReachForce - GetVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            rb.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    private void UpdatePhysicsMovement()
    {
        if (rb == null)
            return;

        if (this is not PlayerAimState)
        {
            UpdateTargetRotation();
            UpdateInputTargetQuaternion();
        }

        if (GetPlayerState().PlayerData.Direction == Vector3.zero)
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
        return Physics.CheckSphere(rb.position + Vector3.up * 0.13f, GetCapsuleCollider().radius / 1.5f, layerMask, QueryTriggerInteraction.Ignore);
    }

    protected CapsuleCollider GetCapsuleCollider()
    {
        if (GetPlayerState().GetPlayerController().GetCharacterManager() == null)
            return null;

        if (GetPlayerState().GetPlayerController().GetCharacterManager().GetCurrentCharacter() == null)
            return null;

        return GetPlayerState().GetPlayerController().GetCharacterManager().GetCurrentCharacter().GetComponent<CapsuleCollider>();
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
        }
        RotateTowardsTargetRotation();

    }

    public void UpdateTargetRotation_Instant(Quaternion quaternion)
    {
        SetTargetRotation(quaternion);
        if (GetPlayerState().PlayerData.CurrentTargetRotation != GetPlayerState().PlayerData.Target_Rotation)
        {
            GetPlayerState().PlayerData.CurrentTargetRotation = GetPlayerState().PlayerData.Target_Rotation;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, GetPlayerState().PlayerData.CurrentTargetRotation.eulerAngles.y, 0f);
        rb.MoveRotation(targetRotation);
    }

    protected void SetTargetRotation(Quaternion quaternion)
    {
        GetPlayerState().PlayerData.Target_Rotation = quaternion;
    }

    protected void RotateTowardsTargetRotation()
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
        rb.AddForce(-playerVerticalVelocity * 3f, ForceMode.Acceleration);
    }
    protected void DecelerateHorizontal()
    {
        Vector3 playerHorizontalVelocity = GetHorizontalVelocity();
        rb.AddForce(-playerHorizontalVelocity * 3f, ForceMode.Acceleration);
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
}
