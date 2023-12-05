using Cinemachine;
using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum LockMovement
{
    Enable,
    Disable
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform CameraLook;
    [SerializeField] PlayerCoordinateAttackManager PlayerCoordinateAttackManager;
    [SerializeField] CameraManager cameraManager;
    private CharacterManager characterManager;
    private StaminaManager staminaManager;
    private Rigidbody rb;
    private LockMovement lockMovement;
    private PlayerState playerState;


    public event Action OnElementalSkillHold;
    public event Action OnE_1Down;
    public delegate bool OnElementalBurst();
    public event Action OnElementalSkillTrigger;
    public event OnElementalBurst OnElementalBurstTrigger;
    public event Action OnInteract;
    public event Action OnChargeHold;
    public event Action OnChargeTrigger;
    public delegate Collider[] onPlungeAttack(Vector3 HitGroundPos);
    public event onPlungeAttack OnPlungeAttack;
    public delegate void onNumsKeyInput(float val);
    public onNumsKeyInput OnNumsKeyInput;
    public onNumsKeyInput OnScroll;
    private ResizeableCollider resizeableCollider;
    private MainUI mainUI;

    public ResizeableCollider GetResizeableCollider()
    {
        return resizeableCollider;
    }
    public PlayerCoordinateAttackManager GetPlayerCoordinateAttackManager()
    {
        return PlayerCoordinateAttackManager;
    }

    public void SetLockMovemnt(LockMovement lockMovement)
    {
        this.lockMovement = lockMovement;
    }

    public LockMovement GetLockMovement()
    {
        return lockMovement;
    }
    public float GetSpeed()
    {
        return playerState.GetPlayerMovementState().GetSpeed();
    }

    public bool isBurstActive()
    {
        if (characterManager.GetCurrentCharacter() == null)
            return false;

        return characterManager.GetCurrentCharacter().GetBurstActive();
    }
    private void Awake()
    {
        lockMovement = LockMovement.Enable;

        characterManager = CharacterManager.GetInstance();
        characterManager.SetPlayerController(this);
        staminaManager = StaminaManager.GetInstance();
        mainUI = MainUI.GetInstance();

        playerState = new PlayerState(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        mainUI = MainUI.GetInstance();
        rb = GetComponent<Rigidbody>();
        resizeableCollider = GetComponent<ResizeableCollider>();
        GetCharacterManager().onCharacterChange += RecalculateSize;
        RecalculateSize(null);
    }

    public CharacterManager GetCharacterManager()
    {
        return characterManager;
    }

    private void RecalculateSize(CharacterData characterData)
    {
        resizeableCollider.Resize();
    }


    public PlayerState GetPlayerState()
    {
        return playerState;
    }

    public PlayerMovementState GetPlayerMovementState()
    {
        return playerState.GetPlayerMovementState();
    }

    private void OnDestroy()
    {
        if (GetCharacterManager())
            GetCharacterManager().onCharacterChange -= RecalculateSize;
    }

    void Update()
    {
        if (rb == null)
            return;

        UpdateControls();
        playerState.Update();
    }


    public Transform GetPlayerOffsetPosition()
    {
        return CameraLook;
    }


    public CapsuleCollider GetCapsuleCollider()
    {
        if (GetCharacterManager() == null)
            return null;

        if (GetCharacterManager().GetCurrentCharacter() == null)
            return null;

        return GetCharacterManager().GetCurrentCharacter().GetComponent<CapsuleCollider>();
    }

    public bool IsAiming()
    {
        return GetPlayerMovementState() is PlayerAimState;
    }

    public void CallPlungeAtk(Vector3 HitPosition)
    {
        OnPlungeAttack?.Invoke(HitPosition);
    }


    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, maxdistance, ~LayerMask.GetMask("Player")))
        {
            return hit.point;
        }
        return origin + direction.normalized * maxdistance;
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
        if (mainUI.isPaused() || Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        OnScroll?.Invoke(Input.mouseScrollDelta.y);

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

    }

    public bool IsMoving()
    {
        return GetPlayerMovementState() is PlayerMovingState;
    }

    public bool CanPerformAction()
    {
        return (GetPlayerMovementState() is PlayerGroundState || GetPlayerMovementState() is PlayerAimState) && 
            GetPlayerMovementState() is not PlayerDashState &&
            !isBurstState();
    }

    public bool isBurstState()
    {
        return GetPlayerMovementState() is PlayerBurstState;
    }
    public void ResetVelocity()
    {
        playerState.GetPlayerMovementState().ResetVelocity();
    }

    public StaminaManager GetStaminaManager()
    {
        return staminaManager;
    }

    public CameraManager GetCameraManager()
    {
        return cameraManager;
    }
    public void UpdateDefaultPosOffsetAndZoom()
    {
        GetCameraManager().CameraDefault();
    }

    public void UpdateAim()
    {
        GetCameraManager().CameraAim();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb == null)
            return;

        playerState.FixedUpdate();
    }

    public Vector3 GetInputDirection()
    {
        return playerState.GetPlayerMovementState().GetInputDirection();
    }
 
    public Rigidbody GetCharacterRB()
    {
        return rb;
    }
}
