using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilState : SwordCharacterState
{
    public April GetApril()
    {
        return (April)GetPlayerCharacters();
    }
    public AprilData aprilData { get { return (AprilData)CommonCharactersData; } }
    public AprilElementalSkillState aprilElementalSkillState;
    public AprilElementalBurstState aprilElementalBurstState;
    public AprilAttackState aprilAttackState;

    public AprilState(Characters Characters) : base(Characters)
    {
        CommonCharactersData = new AprilData(4, GetApril());
        aprilAttackState = new AprilAttackState(this);
        aprilElementalSkillState = new AprilElementalSkillState(this);
        aprilElementalBurstState = new AprilElementalBurstState(this);
        swordIdleState = new AprilIdleState(this);
        ChangeState(swordIdleState);
    }

}
