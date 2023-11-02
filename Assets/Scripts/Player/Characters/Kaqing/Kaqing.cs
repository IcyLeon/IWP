using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaqing : SwordCharacters
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
        SLASH
    }
    private Coroutine ElementalTimerCoroutine;
    private Vector3 ElementalHitPos;
    private KaqingTeleporter elementalOrb;
    private GameObject targetOrb;
    [SerializeField] GameObject ElementalOrbPrefab;
    [SerializeField] GameObject TargetOrbPrefab;
    private float threasHold_Charged;
    private float Range = 8f;
    private ElementalSKill elementalSKill = ElementalSKill.NONE;
    private ElementalBurst elementalBurst = ElementalBurst.First_Phase;
    private Coroutine BurstCoroutine;

    private void Awake()
    {
        threasHold_Charged = 0;
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

        if (elementalSKill != ElementalSKill.THROW)
            UpdateInputTargetQuaternion();

        if (elementalOrb != null)
        {
            if (elementalOrb.GetEnergyOrbMoving())
            {
                UpdateDefaultPosOffsetAndZoom(0.1f);
            }
        }

        Animator.SetBool("isFalling", GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.FALL);
        Animator.SetFloat("Velocity", GetPlayerController().GetSpeed(), 0.15f, Time.deltaTime);
        Animator.SetBool("isGrounded", GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.IDLE);

        UpdateTargetOrb();
        base.Update();
    }

    private void BurstAreaDamage(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, 10f, LayerMask.GetMask("Entity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                damage.TakeDamage(collider.transform.position, new Elements(GetPlayersSO().Elemental), 100f);
            }
        }
    }

    private IEnumerator Burst()
    {
        int TotalHits = 8;
        float TimeInBetweenHits = 2.5f / (TotalHits * 2);
        float HitElapsed = TimeInBetweenHits;
        int CurrentHits = 0;

        Vector3 pos = GetPlayerController().transform.position;

        while (CurrentHits < TotalHits) {

            switch (elementalBurst)
            {
                case ElementalBurst.First_Phase:
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
            Debug.Log(CurrentHits);
            HitElapsed += Time.deltaTime;
            yield return null;
        }
        BurstCoroutine = null;
    }

    protected override bool ElementalBurstTrigger()
    {
        bool canTrigger = base.ElementalBurstTrigger();
        if (canTrigger)
        {
            if (BurstCoroutine != null)
            {
                StopCoroutine(BurstCoroutine);
                BurstCoroutine = null;
            }
            elementalBurst = ElementalBurst.First_Phase;
            BurstCoroutine = StartCoroutine(Burst());
        }

        return canTrigger;
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
        if (!GetCharacterData().CanTriggerESKill())
            return;

        switch (elementalSKill)
        {
            case ElementalSKill.THROW:
                if (threasHold_Charged > 0.1f)
                {
                    if (targetOrb == null)
                        targetOrb = Instantiate(TargetOrbPrefab);
                    UpdateCameraAim();
                    ElementalHitPos = GetRayPosition3D(Camera.main.transform.position, GetVirtualCamera().transform.forward, Range);
                    LookAtDirection(ElementalHitPos - EmitterPivot.position);
                }
                else
                {
                    Characters NearestEnemy = GetNearestCharacters();
                    Vector3 forward;
                    if (NearestEnemy == null)
                    {

                        forward = transform.forward;
                        forward.y = 0;
                        forward.Normalize();
                        ElementalHitPos = GetRayPosition3D(transform.position, forward, Range);
                        ElementalHitPos.y = EmitterPivot.position.y;
                    }
                    else
                    {
                        forward = NearestEnemy.transform.position - transform.position;
                        forward.Normalize();
                        ElementalHitPos = GetRayPosition3D(transform.position, forward, Range);
                        LookAtDirection(ElementalHitPos - EmitterPivot.position);
                    }
                }
                SwordModel.gameObject.SetActive(false);
                Animator.SetBool("2ndSkillAim", true);
                threasHold_Charged += Time.deltaTime;
                break;
        }

    }

    protected override void EKey_1Down()
    {
        if (!GetCharacterData().CanTriggerESKill())
            return;

        switch (elementalSKill)
        {
            case ElementalSKill.NONE:
                elementalSKill = ElementalSKill.THROW;
                break;
        }
    }

    private void ResetThresHold()
    {
        threasHold_Charged = 0;
        SwordModel.gameObject.SetActive(true);
    }

    protected override void ElementalSkillTrigger()
    {
        if (!GetCharacterData().CanTriggerESKill())
            return;

        switch (elementalSKill)
        {
            case ElementalSKill.THROW:
                Animator.SetBool("2ndSkillAim", false);
                StartCoroutine(Shoot());
                break;
            case ElementalSKill.SLASH:
                LookAtDirection(elementalOrb.transform.position - transform.position);
                GetPlayerController().transform.position = elementalOrb.transform.position;
                Animator.SetTrigger("Slash");
                StartElementalTimer(5f);
                ResetThresHold();
                UpdateDefaultPosOffsetAndZoom(0);
                FloatFor(0.5f);
                Destroy(elementalOrb.gameObject);
                break;
        }
    }

    private IEnumerator Shoot()
    {
        if (targetOrb != null)
            Destroy(targetOrb.gameObject);

        while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("2ndSkillThrow") || Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.45f)
        {
            yield return null;
        }

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
        GetPlayerController().StayAfloatFor(sec);
    }
}
