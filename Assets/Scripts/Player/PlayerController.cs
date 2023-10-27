using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum GroundStatus
{
    GROUND,
    AIR
}

public enum PlayerActionStatus
{
    IDLE,
    JUMP,
    FALL
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] float WalkSpeed, SlowMultiplier, ZoomMultiplier;
    [SerializeField] CinemachineVirtualCamera playerCamera, aimCamera;

    private float Speed;
    private float MaxCameraDistance = 5f;
    private float CameraDistance;
    private Rigidbody rb;
    private Vector3 InputDirection;
    private GroundStatus groundStatus;
    private PlayerActionStatus playerActionStatus;

    private Quaternion CurrentTargetRotation, Target_Rotation;
    private float timeToReachTargetRotation;
    private float dampedTargetRotationCurrentVelocity;
    private float dampedTargetRotationPassedTime;

    public delegate void OnMouseScroll(float inputValue);
    public OnMouseScroll onMouseScroll;
    public event Action OnElementalSkillHold;
    public event Action OnE_1Down;
    public event Action OnElementalSkillTrigger;
    public event Action OnElementalBurstTrigger;
    public event Action OnChargeHold;
    public event Action OnChargeTrigger;
    public delegate void onNumsKeyInput(int val);
    public onNumsKeyInput OnNumsKeyInput;

    public delegate void OnPlayerStateChange(PlayerActionStatus PlayerActionStatus);
    public OnPlayerStateChange onPlayerStateChange;

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
        CameraDistance = MaxCameraDistance;
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
        UpdateGrounded();
        UpdateCamera();
  
        if (Input.GetKeyDown(KeyCode.Space) && GetGroundStatus() == GroundStatus.GROUND)
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Sprint();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ResetSpeed();
        }
    }

    public GroundStatus GetGroundStatus()
    {
        return groundStatus;
    }



    private void UpdateCamera()
    {
        if (GetCharacterRB() == null)
            return;

        playerCamera.Follow = GetCharacterRB().transform;
        playerCamera.LookAt = GetCharacterRB().transform;
        aimCamera.Follow = playerCamera.Follow;
        aimCamera.LookAt = playerCamera.LookAt;
    }

    private void UpdateGrounded()
    {
        if (CharacterManager.GetInstance().GetCurrentCharacter() == null)
            return;

        if (Physics.Raycast(rb.position, Vector3.down, 0.1f))
        {
            groundStatus = GroundStatus.GROUND;
            playerActionStatus = PlayerActionStatus.IDLE;
            onPlayerStateChange?.Invoke(playerActionStatus);
        }
        else
        {
            groundStatus = GroundStatus.AIR;
        }
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
        Debug.Log(GetGroundStatus());
        if (GetGroundStatus() != GroundStatus.GROUND)
            return;

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
        playerCamera.gameObject.SetActive(true);
        aimCamera.gameObject.SetActive(false);
    }

    public void UpdateAim()
    {
        playerCamera.gameObject.SetActive(false);
        aimCamera.gameObject.SetActive(true);
    }


    private void GatherInput()
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        float VerticalInput = Input.GetAxisRaw("Vertical");
        float MouseInput = Input.GetAxisRaw("Mouse ScrollWheel");

        CameraDistance += MouseInput * ZoomMultiplier * -1;
        CameraDistance = Mathf.Clamp(CameraDistance, 2f, 5f);

        InputDirection = (GetVirtualCamera().transform.forward * VerticalInput) + (GetVirtualCamera().transform.right * HorizontalInput);
        InputDirection.y = 0;
        InputDirection.Normalize();

    }

    public void UpdateInputTargetQuaternion()
    {
        if (InputDirection == Vector3.zero)
            return;

        Target_Rotation = Quaternion.LookRotation(InputDirection);
    }

    private void UpdateTargetRotation()
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
        UpdatePhysicsMovement();
        LimitFallVelocity();
        UpdateTargetRotation();

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }

        if (IsMovingDown())
        {
            playerActionStatus = PlayerActionStatus.FALL;
        }
    }

    private void ResetSpeed()
    {
        Speed = WalkSpeed;
    }
    private void UpdatePhysicsMovement()
    {
        if (InputDirection == Vector3.zero || GetGroundStatus() == GroundStatus.AIR)
            return;

        rb.AddForce((InputDirection * Speed) - GetHorizontalVelocity() * SlowMultiplier, ForceMode.VelocityChange);
    }

    private void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetVerticalVelocity();
        rb.AddForce(-playerVerticalVelocity * 2f, ForceMode.Acceleration);
    }
    private void DecelerateHorizontal()
    {
        Vector3 playerVerticalVelocity = GetHorizontalVelocity();
        rb.AddForce(-playerVerticalVelocity * 2f, ForceMode.Acceleration);
    }

    private bool IsMovingUp(float minimumVelocity = 0f)
    {
        return GetVerticalVelocity().y > minimumVelocity;
    }

    private bool IsMovingDown(float minimumVelocity = 0f)
    {
        return GetVerticalVelocity().y < minimumVelocity;
    }

    private bool IsMovingHorizontally(float minimumVelocity = 0f)
    {
        Vector2 horizontal = new Vector2(GetHorizontalVelocity().x, GetHorizontalVelocity().z);
        return horizontal.magnitude > minimumVelocity;
    }

    private void Sprint()
    {
        if (GetGroundStatus() == GroundStatus.AIR)
            return;

        Speed = WalkSpeed * 1.5f;
    }

    private void Jump()
    {
        ResetVelocity();
        rb.AddForce(300f * Vector3.up);
        playerActionStatus = PlayerActionStatus.JUMP;
        onPlayerStateChange?.Invoke(playerActionStatus);
    }

    private void LimitFallVelocity()
    {
        float FallSpeedLimit = 45f;
        Vector3 velocity = GetVerticalVelocity();
        if (velocity.y >= -FallSpeedLimit)
        {
            return;
        }

        Vector3 limitVel = new Vector3(0f, -FallSpeedLimit - velocity.y, 0f);
        rb.AddForce(limitVel, ForceMode.VelocityChange);

    }

    private Vector3 GetHorizontalVelocity()
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

    void UpdateDash()
    {

    }
}
