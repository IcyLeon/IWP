using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaqing : Characters
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
    ElementalSKill elementalSKill = ElementalSKill.NONE;

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
                UpdateDefaultPosOffsetAndZoom(1f);
        }
        UpdateTargetOrb();
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
                if (targetOrb == null)
                    targetOrb = Instantiate(TargetOrbPrefab);
                UpdateCameraAim();
                ElementalHitPos = GetRayPosition3D(Camera.main.transform.position, GetVirtualCamera().transform.forward, 10f);
                LookAtDirection(ElementalHitPos - transform.position);
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

    protected override void ElementalSkillTrigger()
    {
        switch(elementalSKill)
        {
            case ElementalSKill.THROW:
                ElementalOrb Orb = Instantiate(ElementalOrbPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<ElementalOrb>();
                elementalOrb = Orb;
                StartCoroutine(elementalOrb.MoveToTargetLocation(ElementalHitPos, 80f));

                elementalSKill = ElementalSKill.SLASH;
                break;
            case ElementalSKill.SLASH:
                LookAtDirection(elementalOrb.transform.position - transform.position);
                transform.position = elementalOrb.transform.position;
                ResetVelocity();
                Destroy(elementalOrb.gameObject);

                elementalSKill = ElementalSKill.NONE;
                break;
        }
    }


}
