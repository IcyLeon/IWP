using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCharacters : PlayerCharacters
{
    [SerializeField] GameObject ArrowPrefab;
    [SerializeField] Transform EmitterPivot;
    [SerializeField] ParticleSystem ChargeUpFinishPrefab;
    private ParticleSystem ChargeUpEmitter;
    private GameObject CrossHair;
    private Elemental CurrentElemental, ShootElement;
    private float OriginalFireSpeed = 1500f, BaseFireSpeed;
    private float ChargedMaxElapsed = 1.5f; // do not change
    private float ChargeElapsed, ShootChargeElapsed;
    private Vector3 Direction, ShootDirection;
    private bool isChargedFinish;
    private float threasHold_Charged;
    private bool isAimHold;
    private float LastClickedTime, AttackRate = 0.35f;

    public Transform GetEmitterPivot()
    {
        return EmitterPivot;
    }
    private void Awake()
    {
        threasHold_Charged = 0;
        isChargedFinish = false;
        CurrentElemental = Elemental.NONE;
        Range = 10f;
    }
    protected override void Update()
    {
        base.Update();
        if (GetPlayerManager() == null)
            return;

        if (GetPlayerManager().isDeadState())
            return;

        if (Input.GetMouseButton(1) && GetPlayerManager().CanAttack())
        {
            UpdateAim();
            isAimHold = true;
        }
        else
        {
            isAimHold = false;
        }

        if (Input.GetMouseButtonUp(1))
            ResetThresHold(0);

        if (Animator)
        {
            Animator.SetFloat("AimVelocityX", GetPlayerManager().GetPlayerController().GetInputDirection().x, 0.1f, Time.deltaTime);
            Animator.SetFloat("AimVelocityZ", GetPlayerManager().GetPlayerController().GetInputDirection().z, 0.1f, Time.deltaTime);
        }
    }

    protected virtual void Fire(Vector3 direction)
    {
        ShootDirection = direction;
    }

    private void FireArrows()
    {
        Arrow ArrowFire = Instantiate(ArrowPrefab, GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<Arrow>();
        Rigidbody ArrowRB = ArrowFire.GetComponent<Rigidbody>();
        ArrowFire.SetElements(new Elements(ShootElement));
        ArrowFire.SetCharacterData(GetCharacterData());
        ArrowFire.transform.rotation = Quaternion.LookRotation(ShootDirection);

        if (!GetPlayerManager().IsAiming())
            BaseFireSpeed = OriginalFireSpeed * 0.7f;
        else
            BaseFireSpeed = OriginalFireSpeed;

        ArrowRB.AddForce(ShootDirection.normalized * BaseFireSpeed * (1 + ShootChargeElapsed));

        DestroyChargeUpEmitter();
        ChargeElapsed = 0;
        isChargedFinish = false;

        BasicAttackTrigger();
    }

    private void UpdateAim()
    {
        if (CrossHair == null)
            CrossHair = Instantiate(AssetManager.GetInstance().GetCrossHair(), AssetManager.GetInstance().GetCanvasGO().transform);

        if (ChargeElapsed < ChargedMaxElapsed)
        {
            ChargeElapsed += Time.deltaTime;
            CurrentElemental = Elemental.NONE;
        }
        else
        {
            if (!isChargedFinish)
            {
                SpawnChargeUpFinish();
                CurrentElemental = GetPlayersSO().Elemental;
                isChargedFinish = true;
            }
        }

        SpawnChargeEmitter();

        UpdateCameraAim();
        Animator.SetBool("IsAiming", true);
        Direction = (GetRayPosition3D(Camera.main.transform.position, Camera.main.transform.forward, 100f) - GetEmitterPivot().transform.position).normalized;
        LookAtDirection(Camera.main.transform.forward);
    }

    private void SpawnChargeUpFinish()
    {
        ParticleSystem ps = Instantiate(ChargeUpFinishPrefab, EmitterPivot).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
    }

    private void SpawnChargeEmitter()
    {
        if (ChargeUpEmitter != null)
            return;

        ChargeUpEmitter = Instantiate(AssetManager.GetInstance().ChargeUpEmitterPrefab, EmitterPivot).GetComponent<ParticleSystem>();
        foreach(ParticleSystem ps in ChargeUpEmitter.GetComponentsInChildren<ParticleSystem>())
        {
            var mainModule = ps.main;
            mainModule.duration = ChargedMaxElapsed;
        }
        ChargeUpEmitter.Play();
    }
    protected override void ChargeHold()
    {
        if (!GetPlayerManager().CanAttack())
            return;

        if (threasHold_Charged > 0.25f)
        {
            UpdateAim();
        }
        else
        {
            Vector3 forward;
            if (NearestEnemy == null)
            {

                forward = transform.forward;
                forward.y = 0;
                forward.Normalize();
                Direction = ((GetPointOfContact() + forward * Range) - GetEmitterPivot().position).normalized;
            }
            else
            {
                forward = NearestEnemy.transform.position - transform.position;
                forward.Normalize();
                Direction = (NearestEnemy.GetPointOfContact() - GetEmitterPivot().position).normalized;
                LookAtDirection(forward);
            }
        }
        ShootElement = CurrentElemental;
        ShootChargeElapsed = ChargeElapsed;
        threasHold_Charged += Time.deltaTime;
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanAttack())
            return;

        if (GetBurstActive())
            return;

        if (!isAimHold)
            ResetThresHold(0.8f);

        if (Time.time - LastClickedTime > AttackRate)
        {
            Fire(Direction);
            Animator.SetTrigger("TriggerAtk");
            LastClickedTime = Time.time;
        }
    }

    private void ResetThresHold(float delay)
    {
        threasHold_Charged = 0;
        ChargeElapsed = 0;
        UpdateDefaultPosOffsetAndZoom(delay);
        DestroyChargeUpEmitter();
        Animator.SetBool("IsAiming", false);
        isChargedFinish = false;
        CurrentElemental = Elemental.NONE;
        if (CrossHair != null)
        {
            Destroy(CrossHair.gameObject);
        }
    }

    private void DestroyChargeUpEmitter()
    {
        if (ChargeUpEmitter)
            Destroy(ChargeUpEmitter.gameObject);
    }

    protected override void OnDisable()
    {
        DestroyChargeUpEmitter();

        base.OnDisable();
    }
}