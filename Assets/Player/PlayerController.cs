using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum PlayerStatus
{
    IDLE,
    JUMP,
    FALL
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed, SlowMultiplier, ZoomMultiplier;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer cinemachineFramingTransposer;

    private float MaxCameraDistance = 5f;
    private float CameraDistance;
    private Rigidbody rb;
    private Vector3 InputDirection;
    private PlayerStatus playerStatus;


    private Quaternion CurrentTargetRotation, Target_Rotation;
    private float timeToReachTargetRotation;
    private float dampedTargetRotationCurrentVelocity;
    private float dampedTargetRotationPassedTime;

    private Coroutine CameraZoomOffsetCoroutine, CameraPosOffsetCoroutine;
    public delegate void OnMouseScroll(float inputValue);
    public OnMouseScroll onMouseScroll;
    public event Action OnElementalSkillHold;
    public event Action OnE_1Down;
    public event Action OnElementalSkillTrigger;
    public event Action OnElementalBurstTrigger;
    public event Action OnChargeHold;
    public event Action OnChargeTrigger;

    public CinemachineVirtualCamera GetVirtualCamera()
    {
        return virtualCamera;
    }


    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = Instantiate(CharacterManager.GetInstance().GetCharacterList()[0].gameObject, transform);
        InventoryManager.GetInstance().SetCurrentEquipCharacter(obj.GetComponent<Characters>());
        InitData();
    }

    void InitData()
    {
        CameraDistance = MaxCameraDistance;
        CharacterManager.GetInstance().SetPlayerController(this);
        cinemachineFramingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        rb = InventoryManager.GetInstance().GetCurrentEquipCharacter().gameObject.GetComponent<Rigidbody>();
        timeToReachTargetRotation = 0.14f;
    }

    public void SetTargetRotation(Quaternion quaternion)
    {
        Target_Rotation = quaternion;
    }

    void Update()
    {
        //rb = InventoryManager.GetInstance().GetCurrentEquipCharacter().gameObject.GetComponent<Rigidbody>();
        GatherInput();
        UpdateControls();
        UpdateGrounded();
        UpdateCamera();
  
        if (Input.GetKeyDown(KeyCode.Space) && playerStatus == PlayerStatus.IDLE)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
    }

    private IEnumerator UpdateCameraOffsetPosition(float ModifierX, float ModifierY, float Speed)
    {
        while (cinemachineFramingTransposer.m_ScreenX != ModifierX)
        {
            if (cinemachineFramingTransposer.m_ScreenX != ModifierX)
                cinemachineFramingTransposer.m_ScreenX = Mathf.Lerp(cinemachineFramingTransposer.m_ScreenX, ModifierX, Time.deltaTime * Speed);

            yield return null;
        }
        while (cinemachineFramingTransposer.m_ScreenX != ModifierX)
        {
            if (cinemachineFramingTransposer.m_ScreenY != ModifierY)
                cinemachineFramingTransposer.m_ScreenY = Mathf.Lerp(cinemachineFramingTransposer.m_ScreenY, ModifierY, Time.deltaTime * Speed);

            yield return null;
        }
        CameraPosOffsetCoroutine = null;
    }

    private IEnumerator UpdateCameraZoomPosition(float CameraDistance, float Speed)
    {
        while (cinemachineFramingTransposer.m_CameraDistance != CameraDistance)
        {
            cinemachineFramingTransposer.m_CameraDistance = Mathf.Lerp(cinemachineFramingTransposer.m_CameraDistance, CameraDistance, Time.deltaTime * Speed);
            yield return null;
        }
        CameraZoomOffsetCoroutine = null;
    }

    public void CameraOffsetPositionAnim(float ModifierX, float ModifierY, float Speed)
    {
        if (CameraPosOffsetCoroutine != null)
            StopCoroutine(CameraPosOffsetCoroutine);

        CameraPosOffsetCoroutine = StartCoroutine(UpdateCameraOffsetPosition(ModifierX, ModifierY, Speed));
    }

    public void CameraZoomOffsetAnim(float CameraDistance, float Speed)
    {
        if (CameraZoomOffsetCoroutine != null)
            StopCoroutine(CameraZoomOffsetCoroutine);

        CameraZoomOffsetCoroutine = StartCoroutine(UpdateCameraZoomPosition(CameraDistance, Speed));
    }


    private void UpdateCamera()
    {
        if (GetCharacterRB() == null)
            return;

        virtualCamera.Follow = GetCharacterRB().transform;
        virtualCamera.LookAt = GetCharacterRB().transform;
    }

    private void UpdateGrounded()
    {
        if (Physics.Raycast(rb.position, Vector3.down, (rb.transform.GetComponent<CapsuleCollider>().height / 2) + 0.05f))
        {
            playerStatus = PlayerStatus.IDLE;
        }

        if (IsMovingDown() && playerStatus == PlayerStatus.JUMP)
        {
            playerStatus = PlayerStatus.FALL;
        }
    }

    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, maxdistance))
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

    private void UpdateControls()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OnE_1Down?.Invoke();
        else if (Input.GetKey(KeyCode.E))
            OnElementalSkillHold?.Invoke();
        else if (Input.GetKeyUp(KeyCode.E))
            OnElementalSkillTrigger?.Invoke();
        else if(Input.GetKey(KeyCode.Q))
            OnElementalBurstTrigger?.Invoke();
        else if(Input.GetMouseButton(0))
            OnChargeHold?.Invoke();
        else if (Input.GetMouseButtonUp(0))
            OnChargeTrigger?.Invoke();

    }


    public void UpdateDefaultPosOffsetAndZoom()
    {
        CameraOffsetPositionAnim(0.5f, 0.5f, 5f); // return to default
        CameraZoomOffsetAnim(5f, 10f);
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
    }

    private void UpdatePhysicsMovement()
    {
        if (InputDirection == Vector3.zero || playerStatus != PlayerStatus.IDLE)
            return;

        rb.AddForce((InputDirection * Speed) - GetHorizontalVelocity() * SlowMultiplier, ForceMode.VelocityChange);
    }

    private void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetVerticalVelocity();
        rb.AddForce(-playerVerticalVelocity * 1.8f, ForceMode.Acceleration);
    }
    private bool IsMovingUp(float minimumVelocity = 0.1f)
    {
        return GetVerticalVelocity().y > minimumVelocity;
    }

    private bool IsMovingDown(float minimumVelocity = 0.1f)
    {
        return GetVerticalVelocity().y < minimumVelocity;
    }


    private void Dash()
    {
        if (playerStatus != PlayerStatus.IDLE)
            return;

        Vector3 forward = rb.transform.forward;
        forward.y = 0;
        forward.Normalize();

        rb.AddForce((forward * Speed * 1.5f) - GetHorizontalVelocity() * SlowMultiplier, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        playerStatus = PlayerStatus.JUMP;
        rb.AddForce(300f * Vector3.up);
    }

    private void LimitFallVelocity()
    {
        float FallSpeedLimit = 50f;
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
        Vector3 vel = rb.velocity;
        vel.y = 0;
        return vel;
    }

    private Vector3 GetVerticalVelocity()
    {
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
