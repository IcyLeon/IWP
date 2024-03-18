using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterState : CharacterState
{
    public CommonCharactersData CommonCharactersData;
    public CharacterDeadState characterDeadState;

    public PlayerCharacterState(Characters Characters) : base(Characters)
    {
        GetPlayerCharacters().GetPlayerManager().GetPlayerElementalSkillandBurstManager().Subscribe(GetPlayerCharacters());
        ResetBasicAttacks();
    }

    public PlayerCharacters GetPlayerCharacters()
    {
        return (PlayerCharacters)Characters;
    }

    protected IPlayerCharactersState GetIPlayerCharactersState()
    {
        return (IPlayerCharactersState)GetCurrentState();
    }

    public void UpdateBurst()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().UpdateBurst();
    }
    public void UpdateElementalSkill()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().UpdateElementalSkill();
    }

    public void ElementalSkillTrigger()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().ElementalSkillTrigger();
    }
    public void ElementalSkillHold()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().ElementalSkillHold();
    }

    public void ElementalBurstTrigger()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().ElementalBurstTrigger();

        GetPlayerCharacters().SetBurstActive(true);
    }

    public void ChargeHold()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().ChargeHold();
    }

    public void ChargeTrigger()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().ChargeTrigger();
    }

    public void ResetBasicAttacks()
    {
        if (CommonCharactersData == null)
            return;

        CommonCharactersData.BasicAttackPhase = 0;

        for (int i = 1; i <= CommonCharactersData.MaxAttackPhase; i++)
        {
            string AtkName = "Attack" + i;
            if (Characters.ContainsParam(Characters.GetAnimator(), AtkName))
                Characters.GetAnimator().SetBool(AtkName, false);
        }

        GetPlayerCharacters().SetisAttacking(false);
    }
}
