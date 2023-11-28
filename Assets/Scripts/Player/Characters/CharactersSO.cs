using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CharactersSO", menuName = "ScriptableObjects/CharactersSO")]
public class CharactersSO : ItemTemplate
{
    public float BaseHP;
    public float BaseATK;
    public float BaseDEF;

    public override Type GetType()
    {
        return typeof(Characters);
    }
}
