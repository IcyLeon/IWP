using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldTextSO", menuName = "ScriptableObjects/WorldTextSO")]
public class WorldTextSO : ScriptableObject
{
    [Serializable]
    public class ElementalTextColor
    {
        public Elemental elemental;
        public Color32 color;
    }
    [SerializeField] ElementalTextColor[] ElementalTextColorList;

    public Color32 GetColor(Elemental elemental)
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
}
