using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementalColorSO", menuName = "ScriptableObjects/ElementalColorSO")]
public class ElementalColorSO : ScriptableObject
{
    [Serializable]
    public class ElementalInfo
    {
        public Elemental elemental;
        public Sprite ElementSprite;
        public Color32 color;
    }

    [Serializable]
    public class ElementalReactionInfo
    {
        public ElementalReactionState elementalReaction;
        public Color32 color;
    }

    [SerializeField] ElementalInfo[] ElementalInfoList;
    [SerializeField] ElementalReactionInfo[] ElementalReactionInfoList;

    public ElementalInfo GetElementalInfo(Elemental elemental)
    {
        for (int i = 0; i < ElementalInfoList.Length; i++)
        {
            ElementalInfo e = ElementalInfoList[i];
            if (e.elemental == elemental)
            {
                return e;
            }
        }
        return null;
    }

    public ElementalReactionInfo GetElementalReactionInfo(ElementalReactionState elementalReaction)
    {
        for (int i = 0; i < ElementalReactionInfoList.Length; i++)
        {
            ElementalReactionInfo E = ElementalReactionInfoList[i];
            if (E.elementalReaction == elementalReaction)
            {
                return E;
            }
        }
        return null;
    }

    public string GetElementalReactionText(ElementalReactionState elementalReaction)
    {
        switch(elementalReaction)
        {
            case ElementalReactionState.OVERCLOCKED:
                return "Overclocked";
            case ElementalReactionState.MELT:
                return "Melt";
            case ElementalReactionState.STUN:
                return "Stun";
        }
        return null;
    }
}
