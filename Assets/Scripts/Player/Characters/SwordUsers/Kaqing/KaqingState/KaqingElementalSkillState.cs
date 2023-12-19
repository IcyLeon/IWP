using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingElementalSkillState : PlayerElementalSkillState
{
    public KaqingState GetKaqingState()
    {
        return (KaqingState)GetPlayerCharacterState();
    }

    public KaqingElementalSkillState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}