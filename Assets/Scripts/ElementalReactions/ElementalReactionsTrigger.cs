using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalReactionsTrigger
{
    private ElementalReactionState elementalReactionState;
    private float ERDamage;

    public ElementalReactionState GetERState()
    {
        return elementalReactionState;
    }

    public void SetERState(ElementalReactionState e)
    {
        elementalReactionState = e;
    }

    public float CalculateERDamage(Characters characters)
    {
        return 0;
    }
}
