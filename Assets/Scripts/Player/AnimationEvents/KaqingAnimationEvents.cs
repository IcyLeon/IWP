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

    private void SpawnESlash()
    {
        AssetManager.GetInstance().SpawnSlashEffect(ElectroSlashPrefab, GetSwordCharacters().GetSwordModel().GetSlashPivot());
        GetKaqing().GetKaqingAimController().DestroyTeleporter();
    }
}
