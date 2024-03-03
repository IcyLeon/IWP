using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberAnimationEvents : BowUsersAnimationEvents
{
    public void Spawn4Arrows()
    {
        GetAmber().Spawn4Arrows();
    }

    private Amber GetAmber()
    {
        return GetBowCharacters() as Amber;
    }
}
