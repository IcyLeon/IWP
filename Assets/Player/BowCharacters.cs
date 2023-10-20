using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCharacters : Characters
{
    private enum AimState
    {
        NONE,
        AIM
    }
    private float BaseFireSpeed;
    [SerializeField] GameObject ArrowPrefab;
    private float BasicAttackFireRate;
    private float ChargeElapsed;
    private float ChargedMaxElapsed;
    private Vector3 Direction;
    private AimState aimState = AimState.NONE;
    private GameObject CrossHair;
    private float threasHold_Charged = 0.1f;

    private void Awake()
    {
        BaseFireSpeed = 100f;
        ChargeElapsed = 0;
        ChargedMaxElapsed = 3f;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        switch(aimState)
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
            AimHold();
        if (Input.GetMouseButtonUp(1))
            ResetThresHold();
    }

    protected void UpdateCharged()
    {
        if (ChargeElapsed < 1)
            ChargeElapsed += Time.deltaTime / ChargedMaxElapsed;
    }

    protected virtual void Fire(Vector3 direction)
    {
        Rigidbody ArrowFire = Instantiate(ArrowPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        ArrowFire.velocity = direction.normalized * BaseFireSpeed * (1 + ChargeElapsed);

        ChargeElapsed = 0;
    }

    protected override void ElementalSkillHold()
    {
    }

    protected override void EKey_1Down()
    {
    }

    protected virtual void AimHold()
    {
        UpdateAim();
    }

    protected override void ElementalBurstTrigger()
    {
    }

    protected override void ElementalSkillTrigger()
    {
    }

    private void UpdateAim()
    {
        UpdateCameraAim();
        aimState = AimState.AIM;
        Vector3 ElementalHitPos = GetRayPosition3D(Camera.main.transform.position, GetVirtualCamera().transform.forward, 100f);
        Direction = (ElementalHitPos - transform.position).normalized;
        LookAtDirection(Direction);
    }

    protected override void ChargeHold()
    {
        if (threasHold_Charged > 0.1f)
        {
            UpdateAim();
        }
        else
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            Direction = forward;
        }
        threasHold_Charged += Time.deltaTime;
    }

    protected override void ChargeTrigger()
    {
        Fire(Direction);
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
