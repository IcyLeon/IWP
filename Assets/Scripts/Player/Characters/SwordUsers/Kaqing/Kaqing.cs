using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaqing : SwordCharacters, ICoordinateAttack
{
    private enum ElementalBurst
    {
        First_Phase,
        Last_Hit,
    }

    private enum ElementalSKill
    {
        NONE,
        THROW,
        TRAVEL,
        SLASH
    }
    private Coroutine ElementalTimerCoroutine;
    private Vector3 ElementalHitPos;
    private KaqingTeleporter elementalOrb;
    private GameObject targetOrb;
    [SerializeField] GameObject ElementalOrbPrefab;
    [SerializeField] GameObject BurstRangeEffectPrefab;
    private ParticleSystem BurstRangeEffect;
    [SerializeField] GameObject TargetOrbPrefab;
    private float threasHold_Charged;
    private ElementalSKill elementalSKill = ElementalSKill.NONE;
    private ElementalBurst elementalBurst = ElementalBurst.First_Phase;
    private Coroutine BurstCoroutine;
    private float UltiRange;
    private float ESkillRange;

    private void Awake()
    {
        threasHold_Charged = 0;
        UltiRange = 8f;
        ESkillRange = 6.5f;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ElementalHitPos = Vector3.zero;
        CurrentElement = Elemental.NONE;
    }

    private IEnumerator ElementalTimer(float timer)
    {
        CurrentElement = GetCharacterData().GetPlayerCharacterSO().Elemental;
        yield return new WaitForSeconds(timer);
        CurrentElement = Elemental.NONE;
        ElementalTimerCoroutine = null;
    }

    private void StartElementalTimer(float timer)
    {
        if (ElementalTimerCoroutine != null)
            StopCoroutine(ElementalTimerCoroutine);

        ElementalTimerCoroutine = StartCoroutine(ElementalTimer(timer));
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (elementalOrb == null && elementalSKill == ElementalSKill.SLASH)
        {
            GetCharacterData().ResetElementalSkillCooldown();
            elementalSKill = ElementalSKill.NONE;
        }

        if (elementalOrb != null)
        {
            if (elementalOrb.GetEnergyOrbMoving())
            {
                UpdateDefaultPosOffsetAndZoom(0.1f);
            }
        }


        if (Animator.GetCurrentAnimatorStateInfo(0).IsName("BurstWait") && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
            Animator.SetBool("IsBurstFinish", true);

        UpdateTargetOrb();
        base.Update();
    }


    private void BurstAreaDamage(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, UltiRange, LayerMask.GetMask("Entity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                if (!damage.IsDead())
                {
                    ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, collider.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
                    Destroy(hitEffect.gameObject, hitEffect.main.duration);
                    damage.TakeDamage(collider.transform.position, new Elements(GetPlayersSO().Elemental), 100f);
                }
            }
        }
    }

    private IEnumerator SpawnEffects()
    {
        yield return new WaitForSeconds(0.18f);
        BurstRangeEffect = Instantiate(BurstRangeEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Vector3 scale = Vector3.one * UltiRange;
        scale.y = 1f;

        BurstRangeEffect.transform.localScale = scale;
        Destroy(BurstRangeEffect.gameObject, BurstRangeEffect.main.duration);

    }
    private IEnumerator Burst()
    {
        StartCoroutine(SpawnEffects());
        yield return new WaitUntil(() => !GetBurstCamera().gameObject.activeSelf);
        GetModel().SetActive(false);
        yield return new WaitForSeconds(0.2f);

        int TotalHits = 10;
        float TimeInBetweenHits = 2.5f / (TotalHits * 2);
        float HitElapsed = TimeInBetweenHits;
        int CurrentHits = 0;

        Vector3 pos = GetPlayerManager().GetPlayerOffsetPosition().position;

        while (CurrentHits < TotalHits) {
            switch (elementalBurst)
            {
                case ElementalBurst.First_Phase:
                    bool Visible = CurrentHits >= TotalHits / 2f;
                    GetModel().SetActive(Visible);
                    isBurstActive = !Visible;
                    Animator.SetBool("IsBurstFinish", Visible);

                    if (CurrentHits >= TotalHits - 1)
                        elementalBurst = ElementalBurst.Last_Hit;

                    if (HitElapsed > TimeInBetweenHits)
                    {
                        BurstAreaDamage(pos);
                        HitElapsed = 0;
                        CurrentHits++;
                    }
                    break;
                case ElementalBurst.Last_Hit:
                    yield return new WaitForSeconds(0.8f);
                    BurstAreaDamage(pos);
                    CurrentHits++;
                    break;
            }
            HitElapsed += Time.deltaTime;
            yield return null;
        }
        BurstCoroutine = null;
    }

    protected override bool ElementalBurstTrigger()
    {
        if (GetPlayerManager().CanPerformAction() && GetPlayerManager().GetPlayerMovementState() is not PlayerAimState)
        {
            bool canTrigger = base.ElementalBurstTrigger();
            if (canTrigger)
            {
                if (BurstCoroutine != null)
                {
                    StopCoroutine(BurstCoroutine);
                    BurstCoroutine = null;
                }
                isBurstActive = true;
                elementalBurst = ElementalBurst.First_Phase;
                GetPlayerManager().GetPlayerController().GetPlayerCoordinateAttackManager().Subscribe(this);
            }
            return canTrigger;
        }

        return false;
    }

    private void UpdateTargetOrb()
    {
        if (targetOrb != null)
        {
            if (elementalSKill == ElementalSKill.THROW)
                targetOrb.transform.position = ElementalHitPos;
        }
    }

    protected override void ElementalSkillHold()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanAttack())
            return;

        if (elementalOrb != null)
            return;

        switch (elementalSKill)
        {
            case ElementalSKill.THROW:
                if (threasHold_Charged > 0.1f)
                {
                    if (targetOrb == null)
                        targetOrb = Instantiate(TargetOrbPrefab);
                    UpdateCameraAim();
                    Vector3 hitPos = (Camera.main.transform.position + Camera.main.transform.forward * ESkillRange) - GetPlayerManager().GetPlayerOffsetPosition().position;
                    ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, hitPos, ESkillRange);
                }
                else
                {
                    SetEHitPos();
                }
                LookAtDirection(ElementalHitPos - EmitterPivot.position);
                SwordModel.gameObject.SetActive(false);
                Animator.SetBool("2ndSkillAim", true);
                threasHold_Charged += Time.deltaTime;
                break;
        }
    }

    protected override void EKey_1Down()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
            return;

        switch (elementalSKill)
        {
            case ElementalSKill.NONE:
                SetEHitPos();
                elementalSKill = ElementalSKill.THROW;
                break;
        }
    }

    private void SetEHitPos()
    {
        Vector3 forward;
        if (NearestEnemy == null)
        {
            forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, forward, ESkillRange);
            ElementalHitPos.y = EmitterPivot.position.y;
        }
        else
        {
            forward = NearestEnemy.GetPointOfContact() - GetPlayerManager().GetPlayerOffsetPosition().position;
            forward.Normalize();
            ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, forward, ESkillRange);
        }
    }

    private void ResetThresHold()
    {
        threasHold_Charged = 0;
        SwordModel.gameObject.SetActive(true);
    }

    protected override void ElementalSkillTrigger()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
            return;

        switch (elementalSKill)
        {
            case ElementalSKill.THROW:
                if (elementalOrb == null)
                {
                    if (targetOrb != null)
                        Destroy(targetOrb.gameObject);

                    Animator.SetBool("2ndSkillAim", false);
                    Animator.SetTrigger("Throw");
                    elementalSKill = ElementalSKill.TRAVEL;
                }
                break;
            case ElementalSKill.TRAVEL:
                break;

            case ElementalSKill.SLASH:
                if (elementalOrb.GetEnergyOrbMoving())
                    return;

                LookAtDirection(elementalOrb.transform.position - transform.position);
                GetPlayerManager().transform.position = elementalOrb.transform.position;
                Animator.SetTrigger("Slash");
                StartElementalTimer(5f);
                ResetThresHold();
                UpdateDefaultPosOffsetAndZoom(0);
                FloatFor(0.5f);
                Destroy(elementalOrb.gameObject);
                break;
        }
    }

    public void ShootTeleportOrb()
    {
        KaqingTeleporter Orb = Instantiate(ElementalOrbPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<KaqingTeleporter>();
        Orb.SetElements(new Elements(GetPlayersSO().Elemental));
        Orb.SetCharacterData(GetCharacterData());
        elementalOrb = Orb;
        StartCoroutine(elementalOrb.MoveToTargetLocation(ElementalHitPos, 50f));
        ResetThresHold();
        elementalSKill = ElementalSKill.SLASH;
    }

    private void FloatFor(float sec)
    {
        //GetPlayerManager().StayAfloatFor(sec);
    }


    public void UpdateCoordinateAttack()
    {
        if (BurstCoroutine == null && isBurstActive)
        {
            BurstCoroutine = StartCoroutine(Burst());
        }
    }

    public bool CoordinateAttackEnded()
    {
        return GetPlayerManager().GetPlayerMovementState() is not PlayerBurstState;
    }

    public bool CoordinateCanShoot()
    {
        return false;
    }
}
