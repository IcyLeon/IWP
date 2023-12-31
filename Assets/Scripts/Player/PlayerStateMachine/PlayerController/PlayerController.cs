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
    [SerializeField] CameraManager cameraManager;
    private PlayerManager playerManager;
    private LockMovement lockMovement;
    private PlayerState playerState;

    public delegate bool OnElemental();
    public event OnElemental OnElementalSkillHold;
    public event OnElemental OnElementalSkillTrigger;
    public event OnElemental OnElementalBurstTrigger;
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


    public void SetLockMovemnt(LockMovement lockMovement)
    {
        this.lockMovement = lockMovement;
    }

    public LockMovement GetLockMovement()
    {
        return lockMovement;
    }
    public float GetAnimationSpeed()
    {
        return playerState.GetPlayerMovementState().GetAnimationSpeed();
    }


    private void Awake()
    {
        lockMovement = LockMovement.Enable;
        playerManager = GetComponent<PlayerManager>();
        playerState = new PlayerState(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        mainUI = MainUI.GetInstance();
        resizeableCollider = GetComponent<ResizeableCollider>();
        RecalculateSize(null);
        GetPlayerManager().onCharacterChange += RecalculateSize;
    }

    public MainUI GetMainUI()
    {
        return mainUI;
    }

    private void RecalculateSize(CharacterData characterData)
    {
        resizeableCollider.Resize();
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }

    public void OnAnimationTransition()
    {
        playerState.OnAnimationTransition();
    }

    void Update()
    {
        if (GetPlayerManager().GetCharacterRB() == null)
            return;

        UpdateControls();
        if (Time.timeScale != 0)
        {
            playerState.Update();
        }
    }

    public CapsuleCollider GetCapsuleCollider()
    {
        if (GetPlayerManager() == null)
            return null;

        if (GetPlayerManager().GetCurrentCharacter() == null)
            return null;

        return GetPlayerManager().GetCurrentCharacter().GetComponent<CapsuleCollider>();
    }


    public void CallPlungeAtk(Vector3 HitPosition)
    {
        OnPlungeAttack?.Invoke(HitPosition);
    }


    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, maxdistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }
        return origin + direction.normalized * maxdistance;
    }

    public RaycastHit[] GetRayPositionAll3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        RaycastHit[] RaycastAll = Physics.RaycastAll(origin, direction.normalized, maxdistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        return RaycastAll;
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

    public PlayerManager GetPlayerManager()
    {
        return playerManager;
    }
    private void UpdateControls()
    {
        if (Time.timeScale == 0 || Input.GetKey(KeyCode.LeftAlt) || mainUI.FallenPanelIsOpen())
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (GetPlayerManager().GetAliveCharacters() == null)
            return;


        OnScroll?.Invoke(Input.mouseScrollDelta.y);

        if (Input.GetKeyDown(KeyCode.F))
            OnInteract?.Invoke();
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
        if (GetPlayerManager().GetCharacterRB() == null)
            return;
        playerState.FixedUpdate();
    }

    public Vector3 GetInputDirection()
    {
        return playerState.GetPlayerMovementState().GetInputDirection();
    }
 
}
