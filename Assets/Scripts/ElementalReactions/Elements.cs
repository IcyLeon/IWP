using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements
{
    private Elemental Elemental;
    private float ElementalEffectTime = 10f;
    private float CurrentElementalEffectTime;
    private bool active;

    public Elemental GetElements()
    {
        return Elemental;
    }

    public void ResetElementalEffect()
    {
        CurrentElementalEffectTime = ElementalEffectTime;
    }

    public void UpdateElementalEffectTime()
    {
        if (CurrentElementalEffectTime > 0) {
            CurrentElementalEffectTime -= Time.deltaTime;
        }
        active = CurrentElementalEffectTime > 0;
    }

    public bool GetActive()
    {
        return active;
    }

    public Elements(Elemental elemental)
    {
        Elemental = elemental;
        active = true;
        CurrentElementalEffectTime = ElementalEffectTime;
    }
}
