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
    private float OriginalFireSpeed = 1500f, BaseFireSpeed;
    private float LastClickedTime, AttackRate = 0.1f;
    private Vector3 Direction;

    public Transform GetEmitterPivot()
    {
        return EmitterPivot;
    }

    private void Awake()
    {
        Range = 6.5f;
    }
    protected override void Update()
    {
        base.Update();

        if (Animator)
        {
            Animator.SetFloat("AimVelocityX", GetPlayerManager().GetPlayerController().GetInputDirection().x, 0.1f, Time.deltaTime);
            Animator.SetFloat("AimVelocityZ", GetPlayerManager().GetPlayerController().GetInputDirection().z, 0.1f, Time.deltaTime);
        }
    }
    public BowCharactersState GetBowCharactersState()
    {
        return (BowCharactersState)PlayerCharacterState;
    }

    private void FireArrows()
    {
        Arrow ArrowFire = Instantiate(ArrowPrefab, GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<Arrow>();
        Rigidbody ArrowRB = ArrowFire.GetComponent<Rigidbody>();
        ArrowFire.SetElements(new Elements(GetBowCharactersState().BowData.CurrentElemental));
        ArrowFire.SetCharacterData(GetCharacterData());
        ArrowFire.transform.rotation = Quaternion.LookRotation(Direction);

        if (!GetPlayerManager().IsAiming())
            BaseFireSpeed = OriginalFireSpeed * 0.7f;
        else
            BaseFireSpeed = OriginalFireSpeed;

        ArrowRB.AddForce(Direction.normalized * BaseFireSpeed * (1 + GetBowCharactersState().BowData.ChargeElapsed));

        DestroyChargeUpEmitter();
        BasicAttackTrigger();
    }

    public override void LaunchBasicAttack()
    {
        if (Time.time - LastClickedTime > AttackRate)
        {
            if (GetBowCharactersState().GetBowControlState() is not BowAimState)
                SetLookAtTarget();
            Animator.SetTrigger("Attack1");
            LastClickedTime = Time.time;
        }
    }

    public void SpawnCrossHair()
    {
        if (CrossHair == null)
            CrossHair = AssetManager.GetInstance().SpawnCrossHair();
    }

    public void SpawnChargeUpFinish()
    {
        ParticleSystem ps = Instantiate(ChargeUpFinishPrefab, EmitterPivot).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
    }

    public void SpawnChargeEmitter()
    {
        if (ChargeUpEmitter != null)
            return;

        ChargeUpEmitter = Instantiate(AssetManager.GetInstance().ChargeUpEmitterPrefab, EmitterPivot).GetComponent<ParticleSystem>();
        foreach(ParticleSystem ps in ChargeUpEmitter.GetComponentsInChildren<ParticleSystem>())
        {
            var mainModule = ps.main;
            mainModule.duration = GetBowCharactersState().BowData.ChargedMaxElapsed;
        }
        ChargeUpEmitter.Play();
    }

    public void InitHitPos_Aim()
    {
        SpawnCrossHair();

        Vector3 hitdir = (Camera.main.transform.position + Camera.main.transform.forward * 100f) - GetPlayerManager().GetPlayerOffsetPosition().position;
        Direction = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, hitdir, 100f) - GetEmitterPivot().position;
        LookAtDirection(Direction);
    }

    public override void SetLookAtTarget()
    {
        Vector3 forward;
        if (NearestTarget == null)
        {
            forward = transform.forward;
            forward.y = 0;
        }
        else
        {
            forward = NearestTarget.GetPointOfContact() - GetPlayerManager().GetPlayerOffsetPosition().position;
            forward.y = 0;
            LookAtDirection(forward);
        }
        forward.Normalize();
        Direction = forward;
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanAttack() || GetBurstActive())
            return;

        base.ChargeTrigger();
    }

    public void BasicAttack()
    {
        if (Time.time - LastClickedTime > AttackRate)
        {
            SetLookAtTarget();
            Animator.SetBool("Attack1", true);
            LastClickedTime = Time.time;
        }
    }

    public void DestroyChargeUpEmitter()
    {
        if (ChargeUpEmitter)
            Destroy(ChargeUpEmitter.gameObject);

        if (GetBowCharactersState() != null)
            GetBowCharactersState().BowData.isChargedFinish = false;
    }

    public void DestroyCrossHair()
    {
        if (CrossHair)
            Destroy(CrossHair);
    }

    protected override void OnDisable()
    {
        DestroyChargeUpEmitter();

        base.OnDisable();
    }
}