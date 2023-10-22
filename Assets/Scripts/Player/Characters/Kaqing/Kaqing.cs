using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaqing : PlayerCharacters
{
    private enum ElementalSKill
    {
        NONE,
        THROW,
        SLASH
    }
    [SerializeField] Transform EmitterPivot;
    private Vector3 ElementalHitPos;
    private ElementalOrb elementalOrb;
    private GameObject targetOrb;
    [SerializeField] GameObject ElementalOrbPrefab;
    [SerializeField] GameObject TargetOrbPrefab;
    private float threasHold_Charged;
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
            elementalSKill = ElementalSKill.NONE;

        if (elementalSKill != ElementalSKill.THROW)
        {
            UpdateInputTargetQuaternion();
        }

        if (elementalOrb != null)
        {
            if (!elementalOrb.GetEnergyOrbMoving())
                UpdateDefaultPosOffsetAndZoom(0.65f);
        }
        UpdateTargetOrb();
        base.Update();
    }

    private void UpdateTargetOrb()
    {
        if (targetOrb != null)
        {
            if (elementalSKill == ElementalSKill.THROW)
                targetOrb.transform.position = ElementalHitPos;
            else
                Destroy(targetOrb.gameObject);
        }
    }

    protected override void ElementalSkillHold()
    {
        switch (elementalSKill)
        {
            case ElementalSKill.THROW:
                if (threasHold_Charged > 0.1f)
                {
                    if (targetOrb == null)
                        targetOrb = Instantiate(TargetOrbPrefab);
                    UpdateCameraAim();
                    ElementalHitPos = GetRayPosition3D(Camera.main.transform.position, GetVirtualCamera().transform.forward, 7.5f);
                    LookAtDirection(ElementalHitPos - transform.position);
                }
                else
                {
                    Vector3 forward = transform.forward;
                    forward.y = 0;
                    forward.Normalize();
                    ElementalHitPos = GetRayPosition3D(Camera.main.transform.position, forward, 7.5f);
                }
                threasHold_Charged += Time.deltaTime;
                break;
        }

    }

    protected override void EKey_1Down()
    {
        switch (elementalSKill)
        {
            case ElementalSKill.NONE:
                elementalSKill = ElementalSKill.THROW;
                break;
        }
    }

    protected override void ElementalBurstTrigger()
    {
    }

    private void ResetThresHold()
    {
        threasHold_Charged = 0;
        UpdateDefaultPosOffsetAndZoom(0);
    }

    protected override void ElementalSkillTrigger()
    {
        switch(elementalSKill)
        {
            case ElementalSKill.THROW:
                ElementalOrb Orb = Instantiate(ElementalOrbPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<ElementalOrb>();
                elementalOrb = Orb;
                StartCoroutine(elementalOrb.MoveToTargetLocation(ElementalHitPos, 50f));
                ResetThresHold();
                elementalSKill = ElementalSKill.SLASH;
                break;
            case ElementalSKill.SLASH:
                LookAtDirection(elementalOrb.transform.position - transform.position);
                GetPlayerController().transform.position = elementalOrb.transform.position;
                ResetVelocity();
                Destroy(elementalOrb.gameObject);

                elementalSKill = ElementalSKill.NONE;
                break;
        }
    }


}
