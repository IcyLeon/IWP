using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingThrowTeleporter : MonoBehaviour
{
    private Vector3 ElementalHitPos;
    [SerializeField] GameObject TeleporterPrefab;
    private Kaqing kaqing;
    private GameObject targetOrb;
    [SerializeField] AimLookAt AimLookAt;
    [SerializeField] GameObject TargetOrbPrefab;
    [SerializeField] Transform EmitterPivot;

    public void SetKaqing(Kaqing kaqing)
    {
        this.kaqing = kaqing;
    }

    public void SetElementalHitPosition(Vector3 pos)
    {
        ElementalHitPos = pos;
        LookAtElementalHitPos();
    }

    public void LookAtElementalHitPos()
    {
        kaqing.LookAtDirection(ElementalHitPos - kaqing.GetPlayerManager().GetPlayerOffsetPosition().position);
    }

    private void ShootTeleportOrb()
    {
        KaqingTeleporter Orb = Instantiate(TeleporterPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<KaqingTeleporter>();
        Orb.SetElements(new Elements(kaqing.GetPlayersSO().Elemental));
        Orb.SetCharacterData(kaqing);
        Orb.SetTargetLoc(ElementalHitPos);
        Orb.OnDestroyOrb += OnDestroyOrb;
        kaqing.GetKaqingState().KaqingData.kaqingTeleporter = Orb;
    }

    public void DestroyTeleporter()
    {
        if (kaqing.GetKaqingState().KaqingData.kaqingTeleporter != null)
            Destroy(kaqing.GetKaqingState().KaqingData.kaqingTeleporter.gameObject);
    }

    private void OnDestroyOrb()
    {
        kaqing.GetCharacterData().ResetElementalSkillCooldown();
    }

    public void UpdateTargetOrb(Vector3 pos)
    {
        if (targetOrb == null)
            targetOrb = Instantiate(TargetOrbPrefab);

        SetElementalHitPosition(pos);
        targetOrb.transform.position = ElementalHitPos;
        AimLookAt.SetTargetPosition(targetOrb.transform.position);
    }

    public void DestroyTargetOrb()
    {
        if (targetOrb != null)
            Destroy(targetOrb);
    }
}
