using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Kaqing : SwordCharacters
{
    private float CurrentElementalSwordFusion;
    [SerializeField] GameObject ElectroSlashPrefab;
    [SerializeField] GameObject BurstRangeEffectPrefab;
    [SerializeField] KaqingAimController KaqingAim;
    private ParticleSystem BurstRangeEffect;
    [SerializeField] GameObject UltiSlashPrefab;
    [SerializeField] GameObject ElectroPlungeAttack;
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

    public KaqingState GetKaqingState()
    {
        return (KaqingState)PlayerCharacterState;
    }

    protected override Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = base.PlungeAttackGroundHit(HitPos);

        if (GetCurrentSwordElemental() != Elemental.NONE)
            AssetManager.GetInstance().SpawnParticlesEffect(HitPos, ElectroPlungeAttack);
        
        return colliders;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        PlayerCharacterState = new KaqingState(this);
        CurrentElementalSwordFusion = 0;
        base.Start();
        UltiRange = 8f;
        CurrentElement = Elemental.NONE;
    }
    protected override void Update()
    {
        base.Update();
        //Debug.Log(GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState());
    }
    protected override void OnCharacterChanged(CharacterData characterData, PlayerCharacters playerCharacters)
    {
        base.OnCharacterChanged(characterData, playerCharacters);
        GetModel().SetActive(true);
    }

    public override void SpawnSlash()
    {
        if (GetCurrentSwordElemental() == Elemental.NONE)
            base.SpawnSlash();
        else
            SpawnElectroSlash();
    }

    public override float GetATK()
    {
        if (GetCurrentSwordElemental() == Elemental.NONE)
            return base.GetATK();
        else
            return base.GetATK() * 1.25f;
    }

    private void SpawnElectroSlash()
    {
        BasicAttackTrigger();
        AssetManager.GetInstance().SpawnSlashEffect(ElectroSlashPrefab, GetSwordModel().GetSlashPivot());
        GetSwordModel().ResetHits();
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

    protected override bool ElementalSkillTrigger()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
            if (!GetPlayerManager().IsSkillCasting())
                return false;

        GetKaqingState().ElementalSkillTrigger();
        return true;
    }

    public KaqingAimController GetKaqingAimController()
    {
        return KaqingAim;
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
        if (IsDead())
            DestroyUltiSlash();

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
