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
    [SerializeField] float WalkSpeed, ZoomMultiplier;
    [SerializeField] Transform CameraLook;
    [SerializeField] CinemachineVirtualCamera playerCamera, aimCamera;
    [SerializeField] PlayerCoordinateAttackManager PlayerCoordinateAttackManager;

    private float Speed;

    private Rigidbody rb;
    private Vector3 Direction;
    private Vector3 InputDirection;
    private PlayerActionStatus playerActionStatus;
    private PlayerGroundStatus playerGroundStatus;

    private Quaternion CurrentTargetRotation, Target_Rotation;
    private float timeToReachTargetRotation;
    private float dampedTargetRotationCurrentVelocity;
    private float dampedTargetRotationPassedTime;

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

    public CinemachineVirtualCamera GetVirtualCamera()
    {
        if (!playerCamera.gameObject.activeSelf)
            return aimCamera;
        else
            return playerCamera;
    }

    public float GetSpeed()
    {
        return Mathf.Pow(GetHorizontalVelocity().magnitude, 0.5f);
    }

    private void Awake()
    {
        CharacterManager.GetInstance().SetPlayerController(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitData();
    }

    void InitData()
    {
        ResetSpeed();
        timeToReachTargetRotation = 0.14f;
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

    void Update()
    {
        if (rb == null)
            return;

        GatherInput();
        UpdateControls();
        UpdateCamera();
        OnFall();
        OnPlunge();
    }

    private void OnFall()
    {
        if (GetPlayerActionStatus() == PlayerActionStatus.JUMP && IsTouchingTerrain())
            playerActionStatus = PlayerActionStatus.FALL;

        if (GetPlayerActionStatus() != PlayerActionStatus.PLUNGE)
        {
            if (IsMovingDown(0f) && playerGroundStatus == PlayerGroundStatus.AIR)
            {
                playerActionStatus = PlayerActionStatus.FALL;
                onPlayerStateChange?.Invoke();
            }
        }
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
                playerActionStatus = PlayerActionStatus.PLUNGE;
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

    private void UpdateCamera()
    {
        if (GetCharacterRB() == null)
            return;

        playerCamera.Follow = CameraLook;
        playerCamera.LookAt = CameraLook;
        aimCamera.Follow = playerCamera.Follow;
        aimCamera.LookAt = playerCamera.LookAt;
    }

    private bool IsTouchingTerrain()
    {
        CapsuleCollider CapsuleCollider = rb.GetComponent<CapsuleCollider>();
        return Physics.CheckSphere(rb.position + Vector3.up * 0.13f, CapsuleCollider.radius / 1.5f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
    }

    public bool IsAiming()
    {
        return aimCamera.gameObject.activeSelf;
    }

    private Vector3 GroundPoint()
    {
        CapsuleCollider CapsuleCollider = rb.GetComponent<CapsuleCollider>();
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
                OnPlungeAttack?.Invoke();
            }
            playerGroundStatus = PlayerGroundStatus.GROUND;
            playerActionStatus = PlayerActionStatus.IDLE;
            onPlayerStateChange?.Invoke();
        }
        else
        {
            playerGroundStatus = PlayerGroundStatus.AIR;
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


    public void UpdateDefaultPosOffsetAndZoom()
    {
        ResetSpeed();
        playerCamera.gameObject.SetActive(true);
        aimCamera.gameObject.SetActive(false);
    }

    public void UpdateAim()
    {
        Speed = WalkSpeed / 2.5f;
        playerCamera.gameObject.SetActive(false);
        aimCamera.gameObject.SetActive(true);
    }


    private void GatherInput()
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        float VerticalInput = Input.GetAxisRaw("Vertical");
        float MouseInput = Input.GetAxisRaw("Mouse ScrollWheel");

        InputDirection = new Vector3(HorizontalInput, 0f, VerticalInput);

        Direction = (GetVirtualCamera().transform.forward * VerticalInput) + (GetVirtualCamera().transform.right * HorizontalInput);
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
        if (Direction == Vector3.zero)
            return;

        rb.AddForce((Direction * Speed) - GetHorizontalVelocity(), ForceMode.VelocityChange);
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
        if (GetPlayerActionStatus() != PlayerActionStatus.IDLE || IsAiming())
            return;

        playerActionStatus = PlayerActionStatus.JUMP;
        onPlayerStateChange?.Invoke();
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
