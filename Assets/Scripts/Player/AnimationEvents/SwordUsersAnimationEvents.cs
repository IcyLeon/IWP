using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordUsersAnimationEvents : PlayerAnimationEvents
{
    [SerializeField] protected GameObject SlashPrefab;
    [SerializeField] protected GameObject PokePrefab;

    private void ToggleOnCanHit()
    {
        if (!GetSwordCharacters().GetSwordModel())
            return;

        GetSwordCharacters().GetSwordModel().SetCanHit(true);
    }

    protected virtual void SpawnSlash()
    {
        if (GetSwordCharacters().GetSwordModel() == null)
            return;
        AssetManager.SpawnSlashEffect(SlashPrefab, GetSwordCharacters().GetSwordModel().GetSlashPivot());
    }
    protected virtual void SpawnPoke()
    {
        if (GetSwordCharacters().GetSwordModel() == null)
            return;
        AssetManager.SpawnSlashEffect(PokePrefab, GetSwordCharacters().GetSwordModel().GetSlashPivot());
    }

    protected override void Attack()
    {
        if (GetSwordCharacters().GetSwordModel() == null)
            return;

        base.Attack();
        GetSwordCharacters().GetSwordModel().ResetHits();
    }

    private void StingAttack()
    {
        Attack();
        SpawnPoke();
    }

    private void SlashAttack()
    {
        Attack();
        SpawnSlash();
    }

    private void ToggleOffCanHit()
    {
        if (!GetSwordCharacters().GetSwordModel())
            return;

        GetSwordCharacters().GetSwordModel().SetCanHit(false);
    }

    public SwordCharacters GetSwordCharacters()
    {
        return playerCharacters as SwordCharacters;
    }
}
