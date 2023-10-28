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
    private Coroutine FireDelayCoroutine;
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

        Animator.SetBool("isFalling", GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.FALL);
        Animator.SetFloat("AimVelocityX", GetPlayerController().GetHorizontalVelocity().x);
        Animator.SetFloat("AimVelocityZ", GetPlayerController().GetHorizontalVelocity().z);
        Animator.SetFloat("Velocity", GetPlayerController().GetSpeed());
        Animator.SetBool("isGrounded", GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.IDLE);

        base.Update();


        if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
                Animator.SetBool("isAttacking", false);
        }
    }

    protected virtual void Fire(Vector3 direction)
    {
        if (FireDelayCoroutine == null)
            FireDelayCoroutine = StartCoroutine(FireDelay(Direction));
    }

    private IEnumerator FireDelay(Vector3 direction)
    {
        yield return new WaitForSeconds(0.2f);
        Arrow ArrowFire = Instantiate(ArrowPrefab, EmitterPivot.transform.position, Quaternion.identity).GetComponent<Arrow>();
        Rigidbody ArrowRB = ArrowFire.GetComponent<Rigidbody>();
        ArrowFire.SetElements(new Elements(CurrentElemental));
        ArrowFire.SetCharacterData(GetCharacterData());

        ArrowRB.AddForce(direction.normalized * BaseFireSpeed * (1 + ChargeElapsed));
        ChargeElapsed = 0;
        FireDelayCoroutine = null;
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
        Animator.SetBool("IsAiming", true);
        aimState = AimState.AIM;
        Direction = (GetRayPosition3D(Camera.main.transform.position, GetVirtualCamera().transform.forward, 100f) - EmitterPivot.transform.position).normalized;
        LookAtDirection(GetVirtualCamera().transform.forward);
    }

    protected override void ChargeHold()
    {
        if (threasHold_Charged > 0.25f)
        {
            UpdateAim();
        }
        else
        {
            Vector3 forward = GetPlayerController().transform.forward;
            forward.y = 0;
            forward.Normalize();
            Direction = (GetRayPosition3D(transform.position, forward, 10f) - EmitterPivot.position).normalized;
        }
        threasHold_Charged += Time.deltaTime;
    }

    protected override void ChargeTrigger()
    {
        if (!isAimHold)
            ResetThresHold();

        if (Animator.GetBool("isAttacking"))
            return;

        Fire(Direction);
        Animator.SetTrigger("TriggerAtk");
        Animator.SetBool("isAttacking", true);
    }

    private void ResetThresHold()
    {
        threasHold_Charged = 0;
        aimState = AimState.NONE;
        UpdateDefaultPosOffsetAndZoom(0);
        Animator.SetBool("IsAiming", false);
        if (CrossHair != null)
        {
            Destroy(CrossHair.gameObject);
        }
    }
}