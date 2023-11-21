using Cinemachine;
using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum PlayerActionStatus
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

public enum PlayerGroundStatus
{
    GROUND,
    AIR
}


public class PlayerController : MonoBehaviour
{
    public enum LockMovement
    {
        Enable,
        Disable
    }

    [SerializeField] float WalkSpeed, ZoomMultiplier;
    [SerializeField] Transform CameraLook;
    [SerializeField] PlayerCoordinateAttackManager PlayerCoordinateAttackManager;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] AnimationCurve SlopeSpeedAngles;
    private CharacterManager characterManager;

    private float Speed, RunningSpeed;
    private float SpeedModifier = 1f;

    private Rigidbody rb;
    private Vector3 Direction;
    private Vector3 InputDirection;
    private PlayerActionStatus playerActionStatus;
    private PlayerGroundStatus playerGroundStatus;


    private Quaternion CurrentTargetRotation, Target_Rotation;
    private float timeToReachTargetRotation;
    private float dampedTargetRotationCurrentVelocity;
    private float dampedTargetRotationPassedTime;
    private LockMovement lockMovement;


    private float StartDashTime;
    private int consecutiveDashesUsed;
    private int ConsecutiveDashesLimitAmount;
    private float TimeToBeConsideredConsecutive;
    private bool CanDash;


    public event Action OnElementalSkillHold;
    public event Action OnE_1Down;
    public delegate bool OnElementalBurst();
    public event Action OnElementalSkillTrigger;
    public event OnElementalBurst OnElementalBurstTrigger;
    public event Action OnInteract;
    public event Action OnDash;
    public event Action OnChargeHold;
    public event Action OnChargeTrigger;
    public delegate Collider[] onPlungeAttack(Vector3 HitGroundPos);
    public event onPlungeAttack OnPlungeAttack;
    public delegate void onNumsKeyInput(float val);
    public onNumsKeyInput OnNumsKeyInput;
    public onNumsKeyInput OnScroll;
    public event Action onPlayerStateChange;
    private Coroutine FloatCoroutine;

    private ResizeableCollider resizeableCollider;
    public PlayerCoordinateAttackManager GetPlayerCoordinateAttackManager()
    {
        return PlayerCoordinateAttackManager;
    }

    public void SetLockMovemnt(LockMovement lockMovement)
    {
        this.lockMovement = lockMovement;
    }

    public float GetSpeed()
    {
        return Mathf.Pow(GetHorizontalVelocity().magnitude, 0.5f);
    }

    private void Awake()
    {
        lockMovement = LockMovement.Enable;
        CharacterManager.GetInstance().SetPlayerController(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetSpeed();
        CanDash = true;
        timeToReachTargetRotation = 0.14f;
        consecutiveDashesUsed = 0;
        ConsecutiveDashesLimitAmount = 2;
        TimeToBeConsideredConsecutive = 1f;
        RunningSpeed = WalkSpeed * 1.3f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        characterManager = CharacterManager.GetInstance();
        rb = GetComponent<Rigidbody>();
        resizeableCollider = GetComponent<ResizeableCollider>();

        characterManager.onCharacterChange += RecalculateSize;
        RecalculateSize(null);
    }

    private void RecalculateSize(CharacterData characterData)
    {
        resizeableCollider.Resize();
    }
    public void SetTargetRotation(Quaternion quaternion)
    {
        Target_Rotation = quaternion;
    }

    public PlayerActionStatus GetPlayerActionStatus()
    {
        return playerActionStatus;
    }

    private void OnDestroy()
    {
        if (characterManager)
            characterManager.onCharacterChange -= RecalculateSize;
    }

    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = SlopeSpeedAngles.Evaluate(angle);
        return slopeSpeedModifier;
    }


    void Update()
    {
        if (rb == null)
            return;

        UpdateSprint();
        GatherInput();
        UpdateControls();
        OnFall();
        OnPlunge();
    }

    private void OnFall()
    {
        if (GetPlayerActionStatus() != PlayerActionStatus.PLUNGE)
        {
            if (IsMovingDown(0f) && GetPlayerGroundStatus() == PlayerGroundStatus.AIR)
            {
                ChangeState(PlayerActionStatus.FALL);
            }
        }
    }

    private void ChangeState(PlayerActionStatus state)
    {
        playerActionStatus = state;
        onPlayerStateChange?.Invoke();
    }

    private void OnPlunge()
    {
        float PlungeAttackRange = 2f;

        if (GetPlayerActionStatus() == PlayerActionStatus.PLUNGE)
            return;

        if (playerGroundStatus == PlayerGroundStatus.AIR)
        {
            if (Input.GetMouseButtonDown(0) && !Physics.Raycast(rb.position, Vector3.down, PlungeAttackRange))
            {
                ChangeState(PlayerActionStatus.PLUNGE);
                StayAfloatFor(0.45f);
            }
        }
    }

    public void StayAfloatFor(float sec)
    {
        if (FloatCoroutine != null)
            StopCoroutine(FloatCoroutine);

        FloatCoroutine = StartCoroutine(FloatFor(sec));
    }

    private void UpdatePlungeAttack()
    {
        if (rb == null)
            return;

        if (GetPlayerActionStatus() == PlayerActionStatus.PLUNGE && rb.useGravity)
        {
            rb.AddForce(Vector3.down * 50f, ForceMode.Acceleration);
        }
    }

    private void Float()
    {
        if (rb == null)
            return;

        if (resizeableCollider == null || GetCapsuleCollider() == null)
            return;

        Vector3 capsuleColliderCenterInWorldSpace = GetCapsuleCollider().bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if (Physics.Raycast(capsuleColliderCenterInWorldSpace, Vector3.down, out RaycastHit hit, resizeableCollider.GetSlopeData().FloatRayDistance))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if (slopeSpeedModifier == 0f)
            {
                SpeedModifier = 1f;
                return;
            }

            SpeedModifier = slopeSpeedModifier;

            float distanceToFloatingPoint = resizeableCollider.GetColliderCenterInLocalSpace().y * GetCharacterRB().transform.localScale.y - hit.distance;

            if (distanceToFloatingPoint == 0f)
            {
                return;
            }

            float amountToLift = distanceToFloatingPoint * resizeableCollider.GetSlopeData().StepReachForce - GetVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            if (GetPlayerActionStatus() != PlayerActionStatus.JUMP)
                rb.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    public Transform GetPlayerOffsetPosition()
    {
        return CameraLook;
    }
    private bool IsTouchingTerrain()
    {
        if (rb == null || GetCapsuleCollider() == null)
            return false;

        return Physics.CheckSphere(rb.position + Vector3.up * 0.13f, GetCapsuleCollider().radius / 1.5f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
    }

    public CapsuleCollider GetCapsuleCollider()
    {
        if (characterManager == null)
            return null;

        if (characterManager.GetCurrentCharacter() == null)
            return null;

        return characterManager.GetCurrentCharacter().GetComponent<CapsuleCollider>();
    }

    public bool IsAiming()
    {
        return cameraManager.GetAimCamera().activeSelf;
    }

    private Vector3 GroundPoint()
    {
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, GetCapsuleCollider().radius / 1.5f))
        {
            return hit.point;
        }
        return default(Vector3);
    }

    private void UpdateGrounded()
    {
        if (rb == null)
            return;

        if (IsTouchingTerrain() && GetPlayerActionStatus() != PlayerActionStatus.JUMP)
        {
            rb.velocity = GetHorizontalVelocity();
            if (GetPlayerActionStatus() == PlayerActionStatus.PLUNGE)
            {
                Vector3 groundcheckpos = GroundPoint();
                if (groundcheckpos != default(Vector3))
                {
                    rb.position = groundcheckpos;
                    ResetVelocity();
                }
                ChangeState(PlayerActionStatus.IDLE);
                OnPlungeAttack?.Invoke(GetCapsuleCollider().bounds.min);
            }
            if (GetPlayerActionStatus() == PlayerActionStatus.FALL)
                ChangeState(PlayerActionStatus.IDLE);
            playerGroundStatus = PlayerGroundStatus.GROUND;
        }
        else
        {
            playerGroundStatus = PlayerGroundStatus.AIR;
        }

        //Debug.Log(playerActionStatus);
    }

    private void UpdateSprint()
    {
        switch(GetPlayerActionStatus())
        {
            case PlayerActionStatus.SPRINTING:
                if (IsAiming() || InputDirection == Vector3.zero || GetPlayerGroundStatus() != PlayerGroundStatus.GROUND)
                {
                    ResetSpeed();
                    ChangeState(PlayerActionStatus.STOPPING);
                    return;
                }
                Speed = RunningSpeed;
                break;
            case PlayerActionStatus.STOPPING:
                ChangeState(PlayerActionStatus.IDLE);
                break;
        }
    }

    public PlayerGroundStatus GetPlayerGroundStatus()
    {
        return playerGroundStatus;
    }

    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, maxdistance, ~LayerMask.GetMask("Player")))
        {
            return hit.point;
        }
        return origin + direction.normalized * maxdistance;
    }

    public void ResetVelocity()
    {
        if (rb == null)
            return;

        rb.velocity = Vector3.zero;
    }

    public IEnumerator FloatFor(float sec)
    {
        ResetVelocity();
        GetCharacterRB().useGravity = false;
        yield return new WaitForSeconds(sec);
        GetCharacterRB().useGravity = true;
        FloatCoroutine = null;
    }

    private int GetInputNums()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown((KeyCode)(48 + i)))
            {
                return i;
            }
        }
        return -1;
    }
    private void UpdateControls()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            OnDash?.Invoke();


        if (Input.GetKeyDown(KeyCode.F))
            OnInteract?.Invoke();
        else if (Input.GetKeyDown(KeyCode.E))
            OnE_1Down?.Invoke();
        else if (GetInputNums() != -1)
            OnNumsKeyInput?.Invoke(GetInputNums());
        else if (Input.GetKey(KeyCode.E))
            OnElementalSkillHold?.Invoke();
        else if (Input.GetKeyUp(KeyCode.E))
            OnElementalSkillTrigger?.Invoke();
        else if (Input.GetKey(KeyCode.Q))
            OnElementalBurstTrigger?.Invoke();
        else if (Input.GetMouseButton(0))
            OnChargeHold?.Invoke();
        else if (Input.GetMouseButtonUp(0))
            OnChargeTrigger?.Invoke();

        //Debug.Log(playerActionStatus);
    }

    public bool IsInMovingState()
    {
        return GetPlayerActionStatus() == PlayerActionStatus.IDLE ||
            GetPlayerActionStatus() == PlayerActionStatus.WALK ||
            GetPlayerActionStatus() == PlayerActionStatus.SPRINTING;
    }

    private IEnumerator PerformDash()
    {
        ChangeState(PlayerActionStatus.DASH);

        Vector3 dir = transform.forward;

        if (Time.time - StartDashTime > TimeToBeConsideredConsecutive)
        {
            StartDashTime = Time.time;
            consecutiveDashesUsed = 0;
        }

        consecutiveDashesUsed++;

        if (Direction != Vector3.zero)
        {
            dir = Direction;
        }

        dir.y = 0;
        dir.Normalize();

        if (consecutiveDashesUsed == ConsecutiveDashesLimitAmount)
        {
            StartCoroutine(DisableDashState(1f));
            consecutiveDashesUsed = 0;
        }

        rb.velocity = dir * 15f;

        yield return new WaitForSeconds(0.28f);
        ChangeState(PlayerActionStatus.SPRINTING);
    }

    public void Dash()
    {
        if (!CanDash || GetPlayerGroundStatus() != PlayerGroundStatus.GROUND || !IsInMovingState() || IsAiming() ||
            lockMovement == LockMovement.Enable)
            return;

        StartCoroutine(PerformDash());
    }

    private IEnumerator DisableDashState(float sec)
    {
        CanDash = false;
        yield return new WaitForSeconds(sec);
        CanDash = true;
    }


    public void UpdateDefaultPosOffsetAndZoom()
    {
        ResetSpeed();
        cameraManager.CameraDefault();
    }

    public void UpdateAim()
    {
        Speed = WalkSpeed / 1.5f;
        cameraManager.CameraAim();
    }


    private void GatherInput()
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        float VerticalInput = Input.GetAxisRaw("Vertical");

        InputDirection = new Vector3(HorizontalInput, 0f, VerticalInput);
        InputDirection.Normalize();

        Direction = (Camera.main.transform.forward * VerticalInput) + (Camera.main.transform.right * HorizontalInput);
        Direction.y = 0;
        Direction.Normalize();

        OnScroll?.Invoke(Input.mouseScrollDelta.y);
    }


    public void SetInputDirection(Vector3 dir)
    {
        Direction = dir;
    }

    public void UpdateInputTargetQuaternion()
    {
        if (Direction == Vector3.zero)
            return;

        Target_Rotation = Quaternion.LookRotation(Direction);
    }

    public void UpdateTargetRotation()
    {
        if (CurrentTargetRotation != Target_Rotation)
        {
            CurrentTargetRotation = Target_Rotation;
            dampedTargetRotationPassedTime = 0f;
        }
        RotateTowardsTargetRotation();

    }
    private void RotateTowardsTargetRotation()
    {
        if (rb == null)
            return;

        if (GetPlayerActionStatus() == PlayerActionStatus.PLUNGE)
            return;

        float currentYAngle = rb.rotation.eulerAngles.y;

        if (currentYAngle == CurrentTargetRotation.eulerAngles.y)
        {
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, CurrentTargetRotation.eulerAngles.y, ref dampedTargetRotationCurrentVelocity, timeToReachTargetRotation - dampedTargetRotationPassedTime);
        dampedTargetRotationPassedTime += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        rb.MoveRotation(targetRotation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb == null)
            return;

        UpdateGrounded();
        UpdatePlungeAttack();
        Float();
        LimitFallVelocity();
        if (IsMovingUp())
        {
            DecelerateVertically();
        }
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }



    private void ResetSpeed()
    {
        Speed = WalkSpeed;
    }
    public void UpdatePhysicsMovement()
    {
        if (rb == null)
            return;

        if (lockMovement == LockMovement.Enable || GetPlayerActionStatus() == PlayerActionStatus.PLUNGE)
            return;

        if (Direction != Vector3.zero)
        {
            if (GetPlayerActionStatus() != PlayerActionStatus.DASH)
            {
                rb.AddForce((Direction * Speed * SpeedModifier) - GetHorizontalVelocity(), ForceMode.VelocityChange);
                if (GetPlayerActionStatus() != PlayerActionStatus.SPRINTING && GetPlayerActionStatus() != PlayerActionStatus.JUMP)
                    ChangeState(PlayerActionStatus.WALK);
            }
        }
        else
        {
            if (GetPlayerActionStatus() == PlayerActionStatus.WALK)
                ChangeState(PlayerActionStatus.STOPPING);
        }
    }

    private void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetVerticalVelocity();
        rb.AddForce(-playerVerticalVelocity * 3f, ForceMode.Acceleration);
    }
    private void DecelerateHorizontal()
    {
        Vector3 playerHorizontalVelocity = GetHorizontalVelocity();
        rb.AddForce(-playerHorizontalVelocity * 3f, ForceMode.Acceleration);
    }

    private bool IsMovingUp(float minimumVelocity = 0f)
    {
        return GetVerticalVelocity().y > minimumVelocity;
    }

    private bool IsMovingDown(float minimumVelocity = 0f)
    {
        return GetVerticalVelocity().y < -minimumVelocity;
    }

    private bool IsMovingHorizontally(float minimumVelocity = 0f)
    {
        Vector2 horizontal = new Vector2(GetHorizontalVelocity().x, GetHorizontalVelocity().z);
        return horizontal.magnitude > minimumVelocity;
    }

    private void Jump()
    {
        if (!IsInMovingState() || IsAiming() || lockMovement == LockMovement.Enable)
            return;

        if (!IsTouchingTerrain())
            return;

        ChangeState(PlayerActionStatus.JUMP);
        ResetVelocity();
        rb.AddForce(8f * Vector3.up, ForceMode.VelocityChange);
    }

    private void LimitFallVelocity()
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

    public Vector3 GetInputDirection()
    {
        return InputDirection;
    }
    public Vector3 GetHorizontalVelocity()
    {
        if (rb == null)
            return Vector3.zero;

        Vector3 vel = rb.velocity;
        vel.y = 0;
        return vel;
    }

    private Vector3 GetVerticalVelocity()
    {
        if (rb == null)
            return Vector3.zero;

        return new Vector3(0f, rb.velocity.y, 0f);
    }
    public Rigidbody GetCharacterRB()
    {
        return rb;
    }
}
