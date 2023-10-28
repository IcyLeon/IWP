using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCharacters : PlayerCharacters
{
    private enum AimState { NONE, AIM }
    
    [SerializeField] GameObject ArrowPrefab;
    [SerializeField] Transform EmitterPivot;
    private GameObject CrossHair;
    private Elemental CurrentElemental;
    private float BaseFireSpeed = 1500f;
    private float ChargedMaxElapsed = 1.5f;
    private float ChargeElapsed;
    private Vector3 Direction;
    private AimState aimState = AimState.NONE;
    private float threasHold_Charged;
    private bool isAimHold;

    private void Awake()
    {
        threasHold_Charged = 0;
        CurrentElemental = Elemental.NONE;
    }

    protected override void Update()
    {
        switch (aimState)
        {
            case AimState.NONE:
                UpdateInputTargetQuaternion();
                break;
            case AimState.AIM:
                if (CrossHair == null)
                    CrossHair = Instantiate(AssetManager.GetInstance().GetCrossHair(), AssetManager.GetInstance().GetCanvasGO().transform);
                break;
        }

        if (Input.GetMouseButton(1))
        {
            UpdateAim();
            isAimHold = true;
        }
        else
        {
            isAimHold = false;
        }

        if (Input.GetMouseButtonUp(1))
            ResetThresHold();

        base.Update();
    }

    protected virtual void Fire(Vector3 direction)
    {
        Debug.Log("Fire");
        Arrow ArrowFire = Instantiate(ArrowPrefab, EmitterPivot.transform.position, Quaternion.identity).GetComponent<Arrow>();
        Rigidbody ArrowRB = ArrowFire.GetComponent<Rigidbody>();
        ArrowFire.SetElements(new Elements(CurrentElemental));
        ArrowFire.SetCharacterData(GetCharacterData());

        ArrowRB.AddForce(direction.normalized * BaseFireSpeed * (1 + ChargeElapsed));
        ChargeElapsed = 0;
    }

    private void UpdateAim()
    {
        if (ChargeElapsed < 1)
        {
            ChargeElapsed += Time.deltaTime / ChargedMaxElapsed;
            CurrentElemental = Elemental.NONE;
        }
        else
        {
            CurrentElemental = GetPlayersSO().Elemental;
        }

        UpdateCameraAim();
        aimState = AimState.AIM;
        Direction = (GetRayPosition3D(Camera.main.transform.position, GetVirtualCamera().transform.forward, 100f) - EmitterPivot.transform.position).normalized;
        LookAtDirection(GetVirtualCamera().transform.forward);
    }

    protected override void ChargeHold()
    {
        if (threasHold_Charged > 0.1f)
        {
            UpdateAim();
        }
        else
        {
            Vector3 forward = GetPlayerController().transform.forward;
            forward.y = 0;
            forward.Normalize();
            Direction = (GetRayPosition3D(transform.position, forward, 5f) - EmitterPivot.position).normalized;
        }
        threasHold_Charged += Time.deltaTime;
    }

    protected override void ChargeTrigger()
    {
        Fire(Direction);
        if (!isAimHold)
            ResetThresHold();
    }

    private void ResetThresHold()
    {
        threasHold_Charged = 0;
        aimState = AimState.NONE;
        UpdateDefaultPosOffsetAndZoom(0);
        if (CrossHair != null)
        {
            Destroy(CrossHair.gameObject);
        }
    }
}