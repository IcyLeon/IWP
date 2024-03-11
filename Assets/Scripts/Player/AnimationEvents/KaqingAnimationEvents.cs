using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingAnimationEvents : SwordUsersAnimationEvents
{
    [SerializeField] GameObject ElectroSlashPrefab;
    [SerializeField] KaqingESlashCollider KaqingESlashCollider;

    private Kaqing GetKaqing()
    {
        return GetSwordCharacters() as Kaqing;
    }
    protected override void Awake()
    {
        base.Awake();
        KaqingESlashCollider.SetKaqing(GetKaqing());
    }

    protected override void SpawnSlash()
    {
        if (GetKaqing().GetCurrentSwordElemental() == Elemental.NONE)
            base.SpawnSlash();
        else
            SpawnElectroSlash();
    }
    protected override void SpawnPoke()
    {
        base.SpawnPoke();
    }

    private void ToggleOnESlash()
    {
        if (!GetSwordCharacters().GetSwordModel())
            return;

        KaqingESlashCollider.SetCanHit(true);
    }

    private void ToggleOffESlash()
    {
        if (!GetSwordCharacters().GetSwordModel())
            return;

        KaqingESlashCollider.SetCanHit(false);
    }
    private void SpawnElectroSlash()
    {
        AssetManager.SpawnSlashEffect(ElectroSlashPrefab, GetSwordCharacters().GetSwordModel().GetSlashPivot());
    }
    private void SpawnESlash()
    {
        SpawnElectroSlash();
        Attack();
        GetKaqing().GetKaqingAimController().DestroyTeleporter();
    }


}
