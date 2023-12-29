using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CharactersSO", menuName = "ScriptableObjects/CharactersSO")]
public class CharactersSO : ItemTemplate
{
    [Serializable]
    public class AscensionInfo
    {
        public float BaseHP;
        public float BaseATK;
        public float BaseDEF;


        public float BaseMaxHP;
        public float BaseMaxATK;
        public float BaseMaxDEF;
    }

    public AscensionInfo[] AscensionInfoList;

    public AscensionInfo GetAscensionInfo(int index)
    {
        int idx = Mathf.Abs(index);

        if (AscensionInfoList.Length == 0 || idx > AscensionInfoList.Length - 1)
            return null;

        return AscensionInfoList[idx];
    }

    public override Type GetTypeREF()
    {
        return typeof(Characters);
    }
}
