using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilState : SwordCharacterState
{
    public April GetApril()
    {
        return (April)GetPlayerCharacters();
    }
    public AprilData aprilData;
    public AprilElementalSkillState aprilElementalSkillState;
    public AprilElementalBurstState aprilElementalBurstState;
    public AprilState(Characters Characters) : base(Characters)
    {
        aprilData = new AprilData();
        aprilElementalSkillState = new AprilElementalSkillState(this);
        aprilElementalBurstState = new AprilElementalBurstState(this);
        swordIdleState = new AprilIdleState(this);
        ChangeState(swordIdleState);
    }

}
