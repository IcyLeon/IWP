using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCharactersState : IState
{
    public void ChargeHold();
    public void ChargeTrigger();
    public void ElementalBurstTrigger();
    public void ElementalSkillTrigger();
    public void ElementalSkillHold();

    public void UpdateOffline();
}
