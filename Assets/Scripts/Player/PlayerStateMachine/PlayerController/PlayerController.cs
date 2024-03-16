using Cinemachine;
using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public enum LockMovement
{
    Enable,
    Disable
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] CameraManager cameraManager;
    [SerializeField] ResizeableCollider resizeableCollider;
    private InputSystem InputSystem;
    private PlayerManager playerManager;
    private LockMovement lockMovement;

    public delegate bool OnElemental();
    public static OnElemental OnElementalSkillHold;
    public static OnElemental OnElementalSkillTrigger;
    public static OnElemental OnElementalBurstTrigger;
    public static Action OnInteract;
    public static Action OnGadgetUse;
    public static Action OnChargeHold;
    public static Action OnChargeTrigger;
    public delegate Collider[] onPlungeAttack(Vector3 HitGroundPos);
    public static onPlungeAttack OnPlungeAttack;
    public delegate void onNumsKeyInput(float val);
    public static onNumsKeyInput OnNumsKeyInput;
    public static onNumsKeyInput OnScroll;
    private MainUI mainUI;

    private void OnEnable()
    {
        InputSystem.Enable();
    }

    private void OnDisable()
    {
        InputSystem.Disable();
    }

    private void Awake()
    {
        InputSystem = new InputSystem();
        lockMovement = LockMovement.Enable;
        playerManager = GetComponent<PlayerManager>();
        PlayerManager.onCharacterChange += RecalculateSize;
    }

    public void DisableInput(InputAction actions, bool condition = true, float sec = 0f)
    {
        StartCoroutine(DisableInputCoroutine(actions, sec, condition));
    }
    private IEnumerator DisableInputCoroutine(InputAction actions, float sec, bool condition)
    {
        actions.Disable();
        yield return new WaitUntil(() => condition);
        yield return new WaitForSeconds(sec);
        actions.Enable();
    }


    // Start is called before the first frame update
    void Start()
    {
        mainUI = MainUI.GetInstance();
    }

    private void OnDestroy()
    {
        PlayerManager.onCharacterChange -= RecalculateSize;
    }

    public ResizeableCollider GetResizeableCollider()
    {
        return resizeableCollider;
    }

    public MainUI GetMainUI()
    {
        return mainUI;
    }

    public InputSystem.PlayerActions GetPlayerActions()
    {
        return InputSystem.Player;
    }

    public void SetLockMovemnt(LockMovement lockMovement)
    {
        this.lockMovement = lockMovement;
    }

    public LockMovement GetLockMovement()
    {
        return lockMovement;
    }


    private void RecalculateSize(CharacterData characterData, PlayerCharacters playerCharacters)
    {
        resizeableCollider.Resize();
    }


    void Update()
    {
        if (GetPlayerManager().GetCharacterRB() == null)
            return;

        UpdateControls();
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


    public static Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, maxdistance, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision")))
        {
            return hit.point;
        }
        return origin + direction.normalized * maxdistance;
    }

    public static RaycastHit[] GetRayPositionAll3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        RaycastHit[] RaycastAll = Physics.RaycastAll(origin, direction.normalized, maxdistance, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"));
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

        if (GetPlayerManager().IsDeadState())
            return;

        OnScroll?.Invoke(Input.mouseScrollDelta.y);

        if (Input.GetKeyDown(KeyCode.F))
            OnInteract?.Invoke();
        else if (Input.GetKeyUp(KeyCode.Z))
            OnGadgetUse?.Invoke();
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

    public Vector3 GetInputDirection()
    {
        return GetPlayerManager().GetPlayerState().GetPlayerMovementState().GetInputDirection();
    }
 
}
