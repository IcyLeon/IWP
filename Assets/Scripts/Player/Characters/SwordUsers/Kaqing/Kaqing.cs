using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerManager;

public class Kaqing : SwordCharacters, ICoordinateAttack
{
    private Coroutine ElementalTimerCoroutine;
    private Vector3 ElementalHitPos;
    private GameObject targetOrb;
    [SerializeField] GameObject ElementalOrbPrefab;
    [SerializeField] GameObject BurstRangeEffectPrefab;
    private ParticleSystem BurstRangeEffect;
    [SerializeField] GameObject TargetOrbPrefab;
    private void Awake()
    {
        PlayerCharacterState = new KaqingState(this);
    }

    private KaqingState GetKaqingState()
    {
        return PlayerCharacterState as KaqingState;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
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

        if (GetKaqingState().GetKaqingControlState() is KaqingESlash)
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

    // Update is called once per frame
    protected override void Update()
    {
        //if (elementalOrb == null && elementalSKill == ElementalSKill.SLASH)
        //{
        //    GetCharacterData().ResetElementalSkillCooldown();
        //    elementalSKill = ElementalSKill.NONE;
        //}

        //if (elementalOrb != null)
        //{
        //    if (elementalOrb.GetEnergyOrbMoving())
        //    {
        //        UpdateDefaultPosOffsetAndZoom(0.1f);
        //    }
        //}


        //if (Animator.GetCurrentAnimatorStateInfo(0).IsName("BurstWait") && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
        //    Animator.SetBool("IsBurstFinish", true);

        //UpdateTargetOrb();

        GetKaqingState().Update();
        base.Update();
    }

    //private void BurstAreaDamage(Vector3 pos)
    //{
    //    Collider[] colliders = Physics.OverlapSphere(pos, UltiRange, LayerMask.GetMask("Entity"));
    //    for (int i = 0; i < colliders.Length; i++)
    //    {
    //        Collider collider = colliders[i];
    //        IDamage damage = collider.GetComponent<IDamage>();
    //        if (damage != null)
    //        {
    //            if (!damage.IsDead())
    //            {
    //                ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, collider.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
    //                Destroy(hitEffect.gameObject, hitEffect.main.duration);
    //                damage.TakeDamage(collider.transform.position, new Elements(GetPlayersSO().Elemental), 100f);
    //            }
    //        }
    //    }
    //}
    public void SpawnHitEffect(IDamage damage)
    {
        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, damage.GetPointOfContact(), Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(hitEffect.gameObject, hitEffect.main.duration);
        damage.TakeDamage(damage.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), 100f);
    }

    private IEnumerator SpawnEffects()
    {
        yield return new WaitForSeconds(0.18f);
        BurstRangeEffect = Instantiate(BurstRangeEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Vector3 scale = Vector3.one * GetKaqingState().KaqingData.UltiRange;
        scale.y = 1f;

        BurstRangeEffect.transform.localScale = scale;
        Destroy(BurstRangeEffect.gameObject, BurstRangeEffect.main.duration);

    }
    //private IEnumerator Burst()
    //{
    //    StartCoroutine(SpawnEffects());
    //    yield return new WaitUntil(() => !GetBurstCamera().gameObject.activeSelf);
    //    GetModel().SetActive(false);
    //    yield return new WaitForSeconds(0.2f);

    //    int TotalHits = 10;
    //    float TimeInBetweenHits = 2.5f / (TotalHits * 2);
    //    float HitElapsed = TimeInBetweenHits;
    //    int CurrentHits = 0;

    //    Vector3 pos = GetPlayerManager().GetPlayerOffsetPosition().position;

    //    while (CurrentHits < TotalHits) {
    //        switch (elementalBurst)
    //        {
    //            case ElementalBurst.First_Phase:
    //                bool Visible = CurrentHits >= TotalHits / 2f;
    //                GetModel().SetActive(Visible);
    //                isBurstActive = !Visible;
    //                Animator.SetBool("IsBurstFinish", Visible);

    //                if (CurrentHits >= TotalHits - 1)
    //                    elementalBurst = ElementalBurst.Last_Hit;

    //                if (HitElapsed > TimeInBetweenHits)
    //                {
    //                    BurstAreaDamage(pos);
    //                    HitElapsed = 0;
    //                    CurrentHits++;
    //                }
    //                break;
    //            case ElementalBurst.Last_Hit:
    //                yield return new WaitForSeconds(0.8f);
    //                BurstAreaDamage(pos);
    //                CurrentHits++;
    //                break;
    //        }
    //        HitElapsed += Time.deltaTime;
    //        yield return null;
    //    }
    //    BurstCoroutine = null;
    //}

    protected override bool ElementalBurstTrigger()
    {
        if (GetPlayerManager().CanPerformAction() && GetPlayerManager().GetPlayerMovementState() is not PlayerAimState)
        {
            bool canTrigger = base.ElementalBurstTrigger();
            if (canTrigger)
            {
                GetKaqingState().GetKaqingControlState().ElementalBurstTrigger();
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

    public void DestroyTeleporter()
    {
        if (GetKaqingState().KaqingData.kaqingTeleporter != null)
            Destroy(GetKaqingState().KaqingData.kaqingTeleporter.gameObject);
    }


    protected override void ElementalSkillHold()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanAttack())
            return;

        //if (elementalOrb != null)
        //    return;

        GetKaqingState().GetKaqingControlState().ElementalSkillHold();

        //switch (elementalSKill)
        //{
        //    case ElementalSKill.THROW:
        //        if (threasHold_Charged > 0.1f)
        //        {
        //            if (targetOrb == null)
        //                targetOrb = Instantiate(TargetOrbPrefab);
        //            UpdateCameraAim();
        //            Vector3 hitPos = (Camera.main.transform.position + Camera.main.transform.forward * ESkillRange) - GetPlayerManager().GetPlayerOffsetPosition().position;
        //            ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, hitPos, ESkillRange);
        //        }
        //        else
        //        {
        //            SetEHitPos();
        //        }
        //        LookAtDirection(ElementalHitPos - EmitterPivot.position);
        //        SwordModel.gameObject.SetActive(false);
        //        Animator.SetBool("2ndSkillAim", true);
        //        threasHold_Charged += Time.deltaTime;
        //        break;
        //}
    }


    public void LookAtElementalHitPos()
    {
        LookAtDirection(ElementalHitPos - EmitterPivot.position);
    }

    protected override void EKey_1Down()
    {
        //if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
        //    return;

        //switch (elementalSKill)
        //{
        //    case ElementalSKill.NONE:
        //        SetEHitPos();
        //        elementalSKill = ElementalSKill.THROW;
        //        break;
        //}
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
        if (NearestEnemy == null)
        {
            forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, forward, GetKaqingState().KaqingData.ESkillRange);
            ElementalHitPos.y = EmitterPivot.position.y;
        }
        else
        {
            forward = NearestEnemy.GetPointOfContact() - GetPlayerManager().GetPlayerOffsetPosition().position;
            forward.Normalize();
            ElementalHitPos = GetRayPosition3D(GetPlayerManager().GetPlayerOffsetPosition().position, forward, GetKaqingState().KaqingData.ESkillRange);
        }
        LookAtElementalHitPos();
    }

    //private void ResetThresHold()
    //{
    //    threasHold_Charged = 0;
    //    SwordModel.gameObject.SetActive(true);
    //}

    protected override void ElementalSkillTrigger()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
            return;

        GetKaqingState().GetKaqingControlState().ElementalSkillTrigger();
        //switch (elementalSKill)
        //{
        //    case ElementalSKill.THROW:
        //        if (elementalOrb == null)
        //        {
        //            if (targetOrb != null)
        //                Destroy(targetOrb.gameObject);

        //            Animator.SetBool("2ndSkillAim", false);
        //            Animator.SetTrigger("Throw");
        //            elementalSKill = ElementalSKill.TRAVEL;
        //        }
        //        break;

        //    case ElementalSKill.SLASH:
        //        if (elementalOrb.GetEnergyOrbMoving())
        //            return;

        //        StartCoroutine(TeleportToLocation());
        //        Destroy(elementalOrb.gameObject);
        //        break;
        //}
    }

    //private IEnumerator TeleportToLocation()
    //{
    //    Vector3 dir = elementalOrb.transform.position - GetPointOfContact();
    //    Vector3 targetposition;

    //    if (Physics.Raycast(GetPointOfContact(), dir.normalized, out RaycastHit hit, ESkillRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
    //    {
    //        targetposition = hit.point;
    //    }
    //    else
    //    {
    //        if (dir.magnitude >= ESkillRange)
    //            targetposition = GetPointOfContact() + dir.normalized * ESkillRange;
    //        else
    //            targetposition = elementalOrb.transform.position;
    //    }

    //    LookAtDirection(dir);
    //    Animator.SetBool("ESlash", true);


    //    GetPlayerManager().GetPlayerController().GetPlayerState().ChangeState(GetPlayerManager().GetPlayerController().GetPlayerState().playerStayAirborneState);
    //    GetPlayerManager().GetPlayerController().GetPlayerState().playerStayAirborneState.StayAirborneFor(0.5f);

    //    while ((GetPointOfContact() - targetposition).magnitude > 1f)
    //    {
    //        GetPlayerManager().GetCharacterRB().position = Vector3.MoveTowards(GetPlayerManager().GetCharacterRB().position, targetposition, Time.deltaTime * 100f);
    //        yield return null;
    //    }

    //    StartElementalTimer(5f);
    //    ResetThresHold();
    //    UpdateDefaultPosOffsetAndZoom(0);
    //}
    private void OnDestroyOrb()
    {
        GetKaqingState().GetKaqing().GetCharacterData().ResetElementalSkillCooldown();
    }

    private void ShootTeleportOrb()
    {
        KaqingTeleporter Orb = Instantiate(ElementalOrbPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<KaqingTeleporter>();
        Orb.SetElements(new Elements(GetPlayersSO().Elemental));
        Orb.SetCharacterData(GetCharacterData());
        Orb.SetTargetLoc(ElementalHitPos);
        Orb.OnDestroyOrb += OnDestroyOrb;
        GetKaqingState().KaqingData.kaqingTeleporter = Orb;

        //elementalOrb = Orb;
        //ResetThresHold();
        //elementalSKill = ElementalSKill.SLASH;
    }
    public void UpdateCoordinateAttack()
    {
        GetKaqingState().UpdateOffline();
    }

    public bool CoordinateAttackEnded()
    {
        return GetPlayerManager().GetPlayerMovementState() is not PlayerBurstState && GetKaqingState().GetKaqingControlState() is not KaqingBurstState;
    }

    public bool CoordinateCanShoot()
    {
        return false;
    }
}
