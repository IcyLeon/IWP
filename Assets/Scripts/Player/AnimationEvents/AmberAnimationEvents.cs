using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberAnimationEvents : BowUsersAnimationEvents
{
    private void Spawn4Arrows()
    {
        GetAmber().Spawn4Arrows();
    }

    private void SpawnAura()
    {
        GetAmber().SpawnAura();
    }
    private Amber GetAmber()
    {
        return GetBowCharacters() as Amber;
    }
}
