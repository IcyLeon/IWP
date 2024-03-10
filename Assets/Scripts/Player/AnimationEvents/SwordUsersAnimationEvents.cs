using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordUsersAnimationEvents : PlayerAnimationEvents
{
    private void ToggleOnCanHit()
    {
        if (!GetSwordCharacters().GetSwordModel())
            return;

        GetSwordCharacters().GetSwordModel().SetCanHit(true);
    }

    protected override void Attack()
    {
        base.Attack();
        GetSwordCharacters().SpawnSlash();
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
