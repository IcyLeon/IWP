using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaqing : SwordCharacters, ICoordinateAttack
{
    private Coroutine ElementalTimerCoroutine;
    private Vector3 ElementalHitPos;
    private GameObject targetOrb;
    [SerializeField] GameObject ElementalOrbPrefab;
    [SerializeField] GameObject BurstRangeEffectPrefab;
    private ParticleSystem BurstRangeEffect;
    [SerializeField] GameObject TargetOrbPrefab;

    private KaqingState GetKaqingState()
    {
        return (KaqingState)PlayerCharacterState;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        PlayerCharacterState = new KaqingState(this);
        base.Start();
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
        base.SpawnSlash();

        if (PlayerCharacterState.GetPlayerControlState() is KaqingESlash)
            GetKaqingState().GetKaqing().DestroyTeleporter();
    }

    private IEnumerator ElementalTimer(float timer)
    {
        CurrentElement = GetCharacterData().GetPlayerCharacterSO().Elemental;
        yield return new WaitForSeconds(timer);
        CurrentElement = Elemental.NONE;
        ElementalTimerCoroutine = null;
    }

    public void StartElementalTimer(float timer)
    {
        if (ElementalTimerCoroutine != null)
            StopCoroutine(ElementalTimerCoroutine);

        ElementalTimerCoroutine = StartCoroutine(ElementalTimer(timer));
    }
    protected override void Update()
    {
        base.Update();
        if (Animator)
        {
            Animator.SetBool("isWalking", GetPlayerManager().IsMoving());
        }

    }

    public void SpawnHitEffect(IDamage damage)
    {
        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, damage.GetPointOfContact(), Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(hitEffect.gameObject, hitEffect.main.duration);
        damage.TakeDamage(damage.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), 100f);
    }

    public void SpawnEffects()
    {
        StartCoroutine(SpawnEffectsCoroutine());
    }

    private IEnumerator SpawnEffectsCoroutine()
    {
        yield return new WaitForSeconds(0.18f);
        BurstRangeEffect = Instantiate(BurstRangeEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Vector3 scale = Vector3.one * GetKaqingState().KaqingData.UltiRange;
        scale.y = 1f;
        BurstRangeEffect.transform.localScale = scale;
        Destroy(BurstRangeEffect.gameObject, BurstRangeEffect.main.duration);

    }

    protected override bool ElementalBurstTrigger()
    {
        if (GetPlayerManager().CanPerformAction() && GetPlayerManager().GetPlayerMovementState() is not PlayerAimState)
        {
            bool canTrigger = base.ElementalBurstTrigger();
            if (canTrigger)
            {
                GetPlayerManager().GetPlayerController().GetPlayerCoordinateAttackManager().Subscribe(this);
            }
            return canTrigger;
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
        LookAtDirection(ElementalHitPos - EmitterPivot.position);
    }

    public void InitElementalSkillHitPos_Aim()
    {
        Vector3 hitdir = (Camera.main.transform.position + Camera.main.transform.forward * GetKaqingState().KaqingData.ESkillRange) - GetPlayerManager().GetPlayerOffsetPosition().position;
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
            ElementalHitPos.y = EmitterPivot.position.y;
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

    public void UpdateCoordinateAttack()
    {
        GetKaqingState().UpdateOffline();
    }

    public bool CoordinateAttackEnded()
    {
        return GetPlayerManager().GetPlayerMovementState() is not PlayerBurstState && PlayerCharacterState.GetPlayerControlState() is not KaqingBurstState;
    }

    public bool CoordinateCanShoot()
    {
        return false;
    }
}
