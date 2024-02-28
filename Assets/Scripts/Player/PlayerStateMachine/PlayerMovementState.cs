using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementState : IState
{
    private PlayerState PlayerState;
    private float WalkSpeed;
    private float Speed;
    private float DecelerateForce = 4.5f;
    private PlayerCharacters playerCharacter;
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

    protected bool IsAttacking()
    {
        return GetPlayerState().GetPlayerMovementState() is PlayerAttackState;
    }

    public virtual void Update()
    {
        GetPlayerState().rb = GetPlayerState().GetPlayerController().GetPlayerManager().GetCharacterRB();
        UpdatePreviousPosition();

        playerCharacter = GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter();
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
        UpdateDash();
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
            dir = GetPlayerState().GetPlayerController().transform.forward;
            if (GetPlayerState().PlayerData.Direction != Vector3.zero)
                dir = GetPlayerState().PlayerData.Direction;
        }
        dir.y = 0f;
        dir.Normalize();

        if (Physics.Raycast(GetPlayerState().GetPlayerController().GetPlayerManager().GetPlayerOffsetPosition().position, dir, horizontalDisCheck, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore))
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
        if (GetPlayerState().rb == null)
            return;

        if (this is not PlayerGroundState)
            return;

        if (CheckIfisAboutToFall())
            return;


        if (Physics.Raycast(GetPlayerState().GetPlayerController().GetPlayerManager().GetPlayerOffsetPosition().position, Vector3.down, out RaycastHit hit, float.MaxValue, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore))
        {
            GetPlayerState().PlayerData.PreviousPosition = hit.point;
        }
    }

    private void UpdatePhysicsMovement()
    {
        if (GetPlayerState().rb == null)
            return;


        if (playerCharacter)
            if (playerCharacter.GetPlayerCharacterState().GetPlayerControlState() is PlayerElementalSkillState || this is PlayerAttackState)
                return;

        if (this is not PlayerAimState)
        {
            UpdateInputTargetQuaternion();
            UpdateTargetRotation();
        }

        if (GetInputDirection() == Vector3.zero)
            return;

        if (GetPlayerState().GetPlayerController().GetLockMovement() == LockMovement.Enable || GetPlayerState().PlayerData.SpeedModifier == 0)
            return;

        GetPlayerState().rb.AddForce((GetPlayerState().PlayerData.Direction * Speed * GetPlayerState().PlayerData.SpeedModifier) - GetHorizontalVelocity(), ForceMode.VelocityChange);
    }

    protected bool IsTouchingTerrain(bool ignoreEnemies = false)
    {
        if (GetPlayerState().rb == null || GetCapsuleCollider() == null)
            return false;

        if (!GetPlayerState().rb.useGravity)
            return false;

        Vector3 capsuleColliderCenterInWorldSpace = GetCapsuleCollider().bounds.center;
        Collider[] Colliders = Physics.OverlapSphere(capsuleColliderCenterInWorldSpace + Vector3.down * (GetPlayerState().GetPlayerController().GetResizeableCollider().GetDefaultColliderData_height() * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepHeightPercentage + GetCapsuleCollider().height / 2f - GetCapsuleCollider().radius / 2f), GetCapsuleCollider().radius / 1.5f, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore);
        List<Collider> colliderCopy = new(Colliders);
        for(int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            IDamage dmg = colliderCopy[i].GetComponent<IDamage>();
            if (dmg != null)
            {
                if (dmg.IsDead())
                    colliderCopy.RemoveAt(i);
            }
        }

        return colliderCopy.Count > 0;
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
        GetPlayerState().rb.MoveRotation(targetRotation);

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
        GetPlayerState().rb.AddForce(-playerVerticalVelocity * DecelerateForce, ForceMode.Acceleration);
    }
    protected void DecelerateHorizontal()
    {
        Vector3 playerHorizontalVelocity = GetHorizontalVelocity();
        GetPlayerState().rb.AddForce(-playerHorizontalVelocity * DecelerateForce, ForceMode.Acceleration);
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
        GetPlayerState().PlayerData.Direction = (Camera.main.transform.forward * GetInputDirection().z) + (Camera.main.transform.right * GetInputDirection().x);
        GetPlayerState().PlayerData.Direction.y = 0;
        GetPlayerState().PlayerData.Direction.Normalize();
    }

    protected void OnDashInput()
    {
        if (!GetPlayerState().GetPlayerController().GetPlayerManager().GetStaminaManager().CanPerformDash() || !CanDash())
            return;

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && GetPlayerState().GetPlayerController().GetPlayerManager().CanPerformAction())
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
