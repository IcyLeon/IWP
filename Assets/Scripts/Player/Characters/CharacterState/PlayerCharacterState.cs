using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterState : CharacterState
{
    public PlayerCharacterState(Characters Characters) : base(Characters)
    {
    }

    public PlayerCharacters GetPlayerCharacters()
    {
        return (PlayerCharacters)Characters;
    }

    protected IPlayerCharactersState GetIPlayerCharactersState()
    {
        return (IPlayerCharactersState)currentState;
    }

    public void UpdateOffline()
    {
        if (GetIPlayerCharactersState() != null)
            GetIPlayerCharactersState().UpdateOffline();
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

    public PlayerControlState GetPlayerControlState()
    {
        PlayerControlState p = GetIPlayerCharactersState() as PlayerControlState;

        return p;
    }
}