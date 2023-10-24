using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementalColorSO", menuName = "ScriptableObjects/ElementalColorSO")]
public class ElementalColorSO : ScriptableObject
{
    [Serializable]
    public class ElementalTextColor
    {
        public Elemental elemental;
        public Color32 color;
    }

    [Serializable]
    public class ElementalReactionTextColor
    {
        public ElementalReactionState elementalReaction;
        public Color32 color;
    }

    [SerializeField] ElementalTextColor[] ElementalTextColorList;
    [SerializeField] ElementalReactionTextColor[] ElementalReactionTextColorList;

    public Color32 GetColor_Elemental(Elemental elemental)
    {
        for (int i = 0; i < ElementalTextColorList.Length; i++)
        {
            ElementalTextColor elementalTextColor = ElementalTextColorList[i];
            if (elementalTextColor.elemental == elemental)
            {
                return elementalTextColor.color;
            }
        }
        return default(Color32);
    }

    public Color32 GetColor_ElementalReaction(ElementalReactionState elementalReaction)
    {
        for (int i = 0; i < ElementalReactionTextColorList.Length; i++)
        {
            ElementalReactionTextColor E = ElementalReactionTextColorList[i];
            if (E.elementalReaction == elementalReaction)
            {
                return E.color;
            }
        }
        return default(Color32);
    }

    public string GetElementalReactionText(ElementalReactionState elementalReaction)
    {
        switch(elementalReaction)
        {
            case ElementalReactionState.OVERLOAD:
                return "Overload";
            case ElementalReactionState.MELT:
                return "Melt";
        }
        return null;
    }
}
