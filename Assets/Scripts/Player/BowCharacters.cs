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
    private float OriginalFireSpeed = 1000f, BaseFireSpeed;
    private float LastClickedTime, AttackRate = 0.05f, ChargedAttackRate = 0.5f;
    private Vector3 Direction;
    private Elemental ShootElemental;

    public Transform GetEmitterPivot()
    {
        return EmitterPivot;
    }

    private void Awake()
    {
        Range = 8f;
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
        ArrowFire.SetElements(new Elements(ShootElemental));
        ArrowFire.SetCharacterData(GetCharacterData());
        ArrowFire.transform.rotation = Quaternion.LookRotation(Direction);

        if (!GetPlayerManager().IsAiming())
            BaseFireSpeed = OriginalFireSpeed * 0.75f;
        else
            BaseFireSpeed = OriginalFireSpeed;

        ArrowRB.AddForce(Direction.normalized * BaseFireSpeed * (1 + GetBowCharactersState().BowData.ChargeElapsed));

        DestroyChargeUpEmitter();
        ShootElemental = Elemental.NONE;
        BasicAttackTrigger();
    }


    public override void LaunchBasicAttack()
    {
        if (Time.time - LastClickedTime > AttackRate)
        {
            SetLookAtTarget();
            Animator.SetTrigger("Attack1");
            LastClickedTime = Time.time;
        }
    }

    public void LaunchChargedAttack()
    {
        if (Time.time - LastClickedTime > ChargedAttackRate)
        {
            ShootElemental = GetBowCharactersState().BowData.CurrentElemental;
            Animator.SetTrigger("Attack1");
            LastClickedTime = Time.time;
        }
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanPerformAction() && GetPlayerManager().GetPlayerMovementState() is not PlayerAimState)
            return;

        GetPlayerCharacterState().ChargeTrigger();
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
        if (CrossHair == null)
            CrossHair = AssetManager.GetInstance().SpawnCrossHair();

        Vector3 hitdir = (Camera.main.transform.position + Camera.main.transform.forward * 50f) - GetPlayerManager().GetPlayerOffsetPosition().position;
        Direction = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, hitdir, 50f) - GetPlayerManager().GetPlayerOffsetPosition().position;
        LookAtDirection(Direction);
    }

    public override void SetLookAtTarget()
    {
        base.SetLookAtTarget();
        Direction = transform.forward;
    }

    public void DestroyChargeUpEmitter()
    {
        if (ChargeUpEmitter)
            Destroy(ChargeUpEmitter.gameObject);

        if (GetBowCharactersState() != null)
        {
            GetBowCharactersState().BowData.isChargedFinish = false;
            GetBowCharactersState().BowData.ChargeElapsed = 0;
        }
    }

    public void DestroyCrossHair()
    {
        if (CrossHair)
            Destroy(CrossHair);
    }

    protected override void OnCharacterChanged(CharacterData c)
    {
        base.OnCharacterChanged(c);
        DestroyCrossHair();
    }
    protected override void OnDisable()
    {
        DestroyChargeUpEmitter();

        base.OnDisable();
    }
}