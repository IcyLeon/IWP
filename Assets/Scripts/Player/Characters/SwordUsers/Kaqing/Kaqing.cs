using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Kaqing : SwordCharacters
{
    private Vector3 ElementalHitPos;
    private float CurrentElementalSwordFusion;
    [SerializeField] GameObject ElectroSlashPrefab;
    [SerializeField] GameObject ElementalOrbPrefab;
    [SerializeField] GameObject BurstRangeEffectPrefab;
    private ParticleSystem BurstRangeEffect;
    private GameObject targetOrb;
    [SerializeField] GameObject TargetOrbPrefab;
    [SerializeField] Transform EmitterPivot;
    [SerializeField] GameObject UltiSlashPrefab;
    private ParticleSystem UltiSlash;

    public void SpawnUltiSlash()
    {
        if (UltiSlash != null)
            DestroyUltiSlash();

        UltiSlash = Instantiate(UltiSlashPrefab, GetPlayerManager().GetPlayerOffsetPosition().position + Vector3.up * 1f, Quaternion.identity).GetComponent<ParticleSystem>();
    }

    public void DestroyUltiSlash()
    {
        if (UltiSlash)
            Destroy(UltiSlash.gameObject);
    }

    private KaqingState GetKaqingState()
    {
        return (KaqingState)PlayerCharacterState;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        PlayerCharacterState = new KaqingState(this);
        CurrentElementalSwordFusion = 0;
        base.Start();
        UltiRange = 8f;
        ElementalHitPos = Vector3.zero;
        CurrentElement = Elemental.NONE;
        GetPlayerManager().onCharacterChange += onCharacterChange;
    }

    private void onCharacterChange(CharacterData characterData)
    {
        GetModel().SetActive(true);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GetPlayerManager().onCharacterChange -= onCharacterChange;
    }
    public override void SpawnSlash()
    {
        if (GetCurrentSwordElemental() == Elemental.NONE)
            base.SpawnSlash();
        else
            SpawnElectroSlash();

        if (PlayerCharacterState.GetPlayerControlState() is KaqingESlash)
            GetKaqingState().GetKaqing().DestroyTeleporter();
    }

    private void SpawnElectroSlash()
    {
        BasicAttackTrigger();
        AssetManager.GetInstance().SpawnSlashEffect(ElectroSlashPrefab, GetSwordModel().GetSlashPivot());
        Collider[] Colliders = Physics.OverlapSphere(GetPlayerManager().GetPlayerOffsetPosition().position + Vector3.up + transform.forward * 2f, 2.8f, LayerMask.GetMask("Entity"));
        foreach (Collider other in Colliders)
        {
            IDamage damageObj = other.GetComponent<IDamage>();
            if (damageObj != null)
            {
                if (!damageObj.IsDead())
                {
                    Vector3 hitPosition = other.transform.position;

                    if (other is MeshCollider meshCollider)
                    {
                        hitPosition = meshCollider.ClosestPointOnBounds(other.transform.position);
                    }

                    ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, hitPosition, Quaternion.identity).GetComponent<ParticleSystem>();
                    Destroy(hitEffect.gameObject, hitEffect.main.duration);
                    damageObj.TakeDamage(damageObj.GetPointOfContact(), new Elements(GetCurrentSwordElemental()), 150f);
                }
            }
        }
    }

    public void StartElementalTimer()
    {
        CurrentElementalSwordFusion = GetPlayersSO().ElementalSkillsTimer;
        GetPlayerManager().GetPlayerElementalSkillandBurstManager().SubscribeSkillsState(this);
    }


    public void SpawnEffects()
    {
        StartCoroutine(SpawnEffectsCoroutine());
    }

    private IEnumerator SpawnEffectsCoroutine()
    {
        yield return new WaitForSeconds(0.18f);
        BurstRangeEffect = Instantiate(BurstRangeEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Vector3 scale = Vector3.one * UltiRange;
        scale.y = 1f;
        BurstRangeEffect.transform.localScale = scale;
        Destroy(BurstRangeEffect.gameObject, BurstRangeEffect.main.duration);

    }

    protected override bool ElementalBurstTrigger()
    {
        if (GetPlayerManager().CanPerformAction() && !GetPlayerManager().IsAiming())
        {
            base.ElementalBurstTrigger();
        }

        return false;
    }

    public void UpdateTargetOrb()
    {
        if (targetOrb == null)
            targetOrb = Instantiate(TargetOrbPrefab);

        targetOrb.transform.position = ElementalHitPos;
    }

    public void DestroyTargetOrb()
    {
        if (targetOrb != null)
            Destroy(targetOrb);
    }

    public override void OnBurstAnimationDone()
    {
        // do nothing
    }

    public void DestroyTeleporter()
    {
        if (GetKaqingState().KaqingData.kaqingTeleporter != null)
            Destroy(GetKaqingState().KaqingData.kaqingTeleporter.gameObject);
    }

    public void LookAtElementalHitPos()
    {
        LookAtDirection(ElementalHitPos - GetPlayerManager().GetPlayerOffsetPosition().position);
    }

    public void InitElementalSkillHitPos_Aim()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        Vector3 hitdir = (ray.origin + ray.direction * GetKaqingState().KaqingData.ESkillRange) - GetPlayerManager().GetPlayerOffsetPosition().position;
        ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, hitdir, GetKaqingState().KaqingData.ESkillRange);
        LookAtElementalHitPos();
    }

    public void InitElementalSkillHitPos_NoAim()
    {
        Vector3 forward;
        if (NearestTarget == null)
        {
            forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, forward, GetKaqingState().KaqingData.ESkillRange);
        }
        else
        {
            forward = NearestTarget.GetPointOfContact() - GetPlayerManager().GetPlayerOffsetPosition().position;
            forward.Normalize();
            ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, forward, GetKaqingState().KaqingData.ESkillRange);
        }
        LookAtElementalHitPos();
    }

    protected override bool ElementalSkillTrigger()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
            if (!GetPlayerManager().IsSkillCasting())
                return false;

        GetKaqingState().ElementalSkillTrigger();
        return true;
    }

    private void OnDestroyOrb()
    {
        GetCharacterData().ResetElementalSkillCooldown();
    }

    private void ShootTeleportOrb()
    {
        KaqingTeleporter Orb = Instantiate(ElementalOrbPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<KaqingTeleporter>();
        Orb.SetElements(new Elements(GetPlayersSO().Elemental));
        Orb.SetCharacterData(GetCharacterData());
        Orb.SetTargetLoc(ElementalHitPos);
        Orb.OnDestroyOrb += OnDestroyOrb;
        GetKaqingState().KaqingData.kaqingTeleporter = Orb;
    }

    public override void UpdateISkills()
    {
        base.UpdateISkills();
        CurrentElementalSwordFusion -= Time.deltaTime;
        CurrentElementalSwordFusion = Mathf.Clamp(CurrentElementalSwordFusion, 0f, GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer);
    }

    public override bool ISkillsEnded()
    {
        if (IsDead())
            return true;

        return CurrentElementalSwordFusion <= 0;
    }
    public override void UpdateEveryTime()
    {
        if (ISkillsEnded() && GetKaqingState().GetPlayerControlState() is not KaqingESlash && GetKaqingState().GetPlayerControlState() is not KaqingTeleportState)
        {
            CurrentElement = Elemental.NONE;
        }
        else
        {
            CurrentElement = GetPlayersSO().Elemental;
        }
    }
    public override bool IBurstEnded()
    {
        if (IsDead())
            return true;

        return GetPlayerManager().GetPlayerMovementState() is not PlayerBurstState && PlayerCharacterState.GetPlayerControlState() is not KaqingBurstState;
    }
}
