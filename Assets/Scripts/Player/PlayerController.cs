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
    private CharacterManager characterManager;

    private float Speed, RunningSpeed;

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
    public event Action OnChargeHold;
    public event Action OnChargeTrigger;
    public delegate Collider[] onPlungeAttack();
    public event onPlungeAttack OnPlungeAttack;
    public delegate void onNumsKeyInput(int val);
    public onNumsKeyInput OnNumsKeyInput;
    public event Action onPlayerStateChange;
    private Coroutine FloatCoroutine;

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
        RunningSpeed = WalkSpeed * 2f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        characterManager = CharacterManager.GetInstance();
        rb = GetComponent<Rigidbody>();

    }

    public void SetTargetRotation(Quaternion quaternion)
    {
        Target_Rotation = quaternion;
    }

    public PlayerActionStatus GetPlayerActionStatus()
    {
        return playerActionStatus;
    }


    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(rb.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
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
        if (GetPlayerActionStatus() == PlayerActionStatus.JUMP && IsTouchingTerrain())
            ChangeState(PlayerActionStatus.FALL);

        if (GetPlayerActionStatus() != PlayerActionStatus.PLUNGE)
        {
            if (IsMovingDown(0f) && playerGroundStatus == PlayerGroundStatus.AIR)
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
        if (GetPlayerActionStatus() == PlayerActionStatus.PLUNGE && rb.useGravity)
        {
            rb.AddForce(Vector3.down * 50f, ForceMode.Acceleration);
        }
    }

    public Transform GetPlayerOffsetPosition()
    {
        return CameraLook;
    }
    private bool IsTouchingTerrain()
    {
        CapsuleCollider CapsuleCollider = characterManager.GetCurrentCharacter().GetComponent<CapsuleCollider>();
        return Physics.CheckSphere(rb.position + Vector3.up * 0.13f, CapsuleCollider.radius / 1.5f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
    }

    public bool IsAiming()
    {
        return cameraManager.GetAimCamera().activeSelf;
    }

    private Vector3 GroundPoint()
    {
        CapsuleCollider CapsuleCollider = characterManager.GetCurrentCharacter().GetComponent<CapsuleCollider>();
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, CapsuleCollider.radius / 1.5f))
        {
            return hit.point;
        }
        return default(Vector3);
    }

    private void UpdateGrounded()
    {
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
            }
            playerGroundStatus = PlayerGroundStatus.GROUND;
            if (GetPlayerActionStatus() == PlayerActionStatus.FALL)
                ChangeState(PlayerActionStatus.IDLE);
        }
        else
        {
            playerGroundStatus = PlayerGroundStatus.AIR;
        }

        Debug.Log(playerActionStatus);
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
            Dash();

        if (Input.GetKeyDown(KeyCode.E))
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

        rb.velocity = dir * 35f;

        yield return new WaitForSeconds(0.28f);
        ChangeState(PlayerActionStatus.SPRINTING);
    }

    private void Dash()
    {
        if (!CanDash || GetPlayerGroundStatus() != PlayerGroundStatus.GROUND || !IsInMovingState() || IsAiming())
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
        float MouseInput = Input.GetAxisRaw("Mouse ScrollWheel");

        InputDirection = new Vector3(HorizontalInput, 0f, VerticalInput);
        InputDirection.Normalize();

        Direction = (Camera.main.transform.forward * VerticalInput) + (Camera.main.transform.right * HorizontalInput);
        Direction.y = 0;
        Direction.Normalize();

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
        UpdateGrounded();
        LimitFallVelocity();
        UpdatePlungeAttack();

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
        if (GetPlayerActionStatus() != PlayerActionStatus.FALL && GetPlayerActionStatus() != PlayerActionStatus.PLUNGE)
            rb.velocity = AdjustVelocityToSlope(rb.velocity);

        if (lockMovement == LockMovement.Enable || GetPlayerActionStatus() == PlayerActionStatus.PLUNGE)
            return;

        if (Direction != Vector3.zero)
        {
            if (playerActionStatus != PlayerActionStatus.DASH)
            {
                rb.AddForce((Direction * Speed) - GetHorizontalVelocity(), ForceMode.VelocityChange);
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
