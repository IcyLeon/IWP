using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalReactionsTrigger
{
    private ElementalReactionState elementalReactionState;

    public ElementalReactionState GetERState()
    {
        return elementalReactionState;
    }

    public void SetERState(ElementalReactionState e)
    {
        elementalReactionState = e;
    }

    public float CalculateERDamage(IDamage IDamage)
    {
        if (IDamage == null)
            return 0f;

        float ERBonus = 0f;
        float EMBonus = 0f;
        float EMValue = IDamage.GetEM();
        switch(elementalReactionState)
        {
            case ElementalReactionState.STUN:
                ERBonus = 0.5f;
                EMBonus = 2.78f * (EMValue / (EMValue + 1400)) * 0.01f;
                break;
            case ElementalReactionState.OVERCLOCKED:
                ERBonus = 2f;
                EMBonus = 2.78f * (EMValue / (EMValue + 1400)) * 0.01f; 
                break;
            case ElementalReactionState.MELT:
                ERBonus = 2f;
                EMBonus = 16f * (EMValue / (EMValue + 2000)) * 0.01f;
                break;
        }
        float ReactionMultiplier = 1f + EMBonus + ERBonus;

        return IDamage.GetATK() * ReactionMultiplier;
    }
}
