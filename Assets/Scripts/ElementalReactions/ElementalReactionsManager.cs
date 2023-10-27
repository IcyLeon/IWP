using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementalReactionState
{
    OVERCLOCKED,
    MELT,
    STUN
}

[Serializable]
public class ElementalReactionsInfo
{
    public Elemental[] elementals;
    public ElementalReactionState elementalReactionState;
}

public class ElementalReactionsManager : MonoBehaviour
{
    private static ElementalReactionsManager instance;
    [SerializeField] ElementalReactionsInfo[] elementalReactionsInfo;
    [SerializeField] ElementalColorSO elementalColorSO;

    public static ElementalReactionsManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public ElementalColorSO GetElementalColorSO()
    {
        return elementalColorSO;
    }

    public ElementalReactionsInfo GetElementalReactionState(List<Elements> Elementals)
    {
        if (Elementals.Count == 0)
            return null;

        for (int i = 0; i < elementalReactionsInfo.Length; i++)
        {
            int FoundCount = 0;
            List<Elements> ElementalsCopy = new List<Elements>(Elementals);

            ElementalReactionsInfo info = elementalReactionsInfo[i];
            for (int j = 0; j < info.elementals.Length; j++)
            {
                Elemental elemental = info.elementals[j];
                for (int z = 0; z < ElementalsCopy.Count; z++)
                {
                    if (ElementalsCopy[z].GetElements() == elemental)
                    {
                        FoundCount++;
                        ElementalsCopy.RemoveAt(z);
                    }
                }
            }
            if (FoundCount >= Elementals.Count && info.elementals.Length == Elementals.Count)
            {
                return info;
            }
        }
        return null;
    }

    public string GetElementalReactionText(ElementalReactionState e)
    {
        if (elementalColorSO == null)
            return null;

        return elementalColorSO.GetElementalReactionText(e);
    }
}
