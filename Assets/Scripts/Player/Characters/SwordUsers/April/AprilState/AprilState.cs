using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilState : SwordCharacterState
{
    public April GetApril()
    {
        return (April)GetPlayerCharacters();
    }

    public AprilElementalSkillState aprilElementalSkillState;

    public AprilState(Characters Characters) : base(Characters)
    {
        aprilElementalSkillState = new AprilElementalSkillState(this);
        swordIdleState = new AprilIdleState(this);
        ChangeState(swordIdleState);
    }

}
