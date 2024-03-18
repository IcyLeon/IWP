using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BowCharacters : PlayerCharacters
{
    [SerializeField] Transform EmitterPivot;
    [SerializeField] AimController AimController;
    [SerializeField] ParticleSystem ChargeUpFinishPrefab;
    [SerializeField] BowSoundSO BowSoundSO;
    private ParticleSystem ChargeUpEmitter;

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

        if (Time.timeScale == 0)
            return;

        if (Animator)
        {
            Animator.SetFloat("AimVelocityX", GetPlayerManager().GetPlayerController().GetInputDirection().x, 0.1f, Time.deltaTime);
            Animator.SetFloat("AimVelocityZ", GetPlayerManager().GetPlayerController().GetInputDirection().y, 0.1f, Time.deltaTime);
        }
    }
    public BowCharacterState GetBowCharactersState()
    {
        return (BowCharacterState)PlayerCharacterState;
    }

    public BowSoundSO GetBowSoundSO()
    {
        return BowSoundSO;
    }

    protected override Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = base.PlungeAttackGroundHit(HitPos);

        AssetManager.GetInstance().SpawnParticlesEffect(AssetManager.GetInstance().PlungeParticlesEffect, HitPos);

        foreach (Collider collider in colliders)
        {
            IDamage damageObject = collider.gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), new Elements(Elemental.NONE), GetATK() * 2.5f, this);
            }
        }
        return colliders;
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanPerformAction() && GetPlayerManager().GetPlayerMovementState() is not PlayerAimState)
            return;

        GetPlayerCharacterState().ChargeTrigger();
    }


    public void SpawnChargeUpFinish()
    {
        GetSoundManager().PlaySFXSound(BowSoundSO.BowChargeUpAudioClip);
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
            mainModule.duration = GetBowCharactersState().GetBowData().ChargedMaxElapsed;
        }
        ChargeUpEmitter.Play();
    }

    public AimController GetAimController()
    {
        return AimController;
    }
    public void DestroyChargeUpEmitter()
    {
        if (ChargeUpEmitter)
            Destroy(ChargeUpEmitter.gameObject);

        if (GetBowCharactersState() != null)
        {
            GetBowCharactersState().GetBowData().isChargedFinish = false;
            GetBowCharactersState().GetBowData().ChargeElapsed = 0;
        }
    }

    protected override void OnDisable()
    {
        DestroyChargeUpEmitter();

        base.OnDisable();
    }
}