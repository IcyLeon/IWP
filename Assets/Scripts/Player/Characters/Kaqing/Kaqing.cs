using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaqing : SwordCharacters
{
    private enum ElementalOrbState 
    { 
        IDLE,
        MOVING
    }

    private enum ElementalSKill
    {
        NONE,
        THROW,
        SLASH
    }
    private Vector3 ElementalHitPos;
    private KaqingTeleporter elementalOrb;
    private GameObject targetOrb;
    [SerializeField] GameObject ElementalOrbPrefab;
    [SerializeField] GameObject TargetOrbPrefab;
    private float threasHold_Charged;
    private float Range = 8f;

    ElementalOrbState elementalOrbState = ElementalOrbState.IDLE;
    ElementalSKill elementalSKill = ElementalSKill.NONE;

    private void Awake()
    {
        threasHold_Charged = 0;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ElementalHitPos = Vector3.zero;
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
            if (!elementalOrb.GetEnergyOrbMoving() && elementalOrbState == ElementalOrbState.MOVING)
            {
                UpdateDefaultPosOffsetAndZoom(0.1f);
                elementalOrbState = ElementalOrbState.IDLE;
            }
        }

        Animator.SetBool("isFalling", GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.FALL);
        Animator.SetFloat("Velocity", GetPlayerController().GetSpeed());
        Animator.SetBool("isGrounded", GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.IDLE);

        UpdateTargetOrb();
        base.Update();
    }

    //protected override void FixedUpdate()
    //{
    //    if (elementalSKill != ElementalSKill.THROW)
    //        base.FixedUpdate();
    //}

    protected override bool ElementalBurstTrigger()
    {
        return base.ElementalBurstTrigger();
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
        elementalOrbState = ElementalOrbState.MOVING;
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
                ResetThresHold();
                UpdateDefaultPosOffsetAndZoom(0);
                StartCoroutine(FloatFor(0.5f));
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

    private IEnumerator FloatFor(float sec)
    {
        ResetVelocity();
        GetPlayerController().GetCharacterRB().useGravity = false;
        yield return new WaitForSeconds(sec);
        GetPlayerController().GetCharacterRB().useGravity = true;
    }
}
