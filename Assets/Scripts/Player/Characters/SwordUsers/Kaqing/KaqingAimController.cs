using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingAimController : AimController
{
    [SerializeField] GameObject TeleporterPrefab;
    private GameObject targetOrb;
    [SerializeField] GameObject TargetOrbPrefab;
    [SerializeField] Transform EmitterPivot;

    private Kaqing GetKaqing()
    {
        return PlayerCharactersRef as Kaqing;
    }

    private void ShootTeleportOrb()
    {
        KaqingTeleporter Orb = Instantiate(TeleporterPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<KaqingTeleporter>();
        Orb.SetElements(new Elements(PlayerCharactersRef.GetPlayersSO().Elemental));
        Orb.SetCharacterData(PlayerCharactersRef);
        Orb.SetTargetLoc(GetAimTargetPosition());
        Orb.OnDestroyOrb += OnDestroyOrb;
        GetKaqing().GetKaqingState().KaqingData.kaqingTeleporter = Orb;
    }

    public void DestroyTeleporter()
    {
        if (GetKaqing().GetKaqingState().KaqingData.kaqingTeleporter != null)
            Destroy(GetKaqing().GetKaqingState().KaqingData.kaqingTeleporter.gameObject);
    }

    private void OnDestroyOrb(PlayerCharacters PlayerCharacters)
    {
        PlayerCharacters.GetCharacterData().ResetElementalSkillCooldown();
    }

    public void UpdateTargetOrb(Vector3 pos)
    {
        if (targetOrb == null)
            targetOrb = Instantiate(TargetOrbPrefab);

        SetAimTargetPosition(pos);
        targetOrb.transform.position = GetAimTargetPosition();
    }

    public void DestroyTargetOrb()
    {
        if (targetOrb != null)
            Destroy(targetOrb);
    }
}
