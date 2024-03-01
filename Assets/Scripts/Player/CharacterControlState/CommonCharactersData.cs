using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonCharactersData
{
    public int BasicAttackPhase;
    public int MaxAttackPhase;
    public float LastClickedTime;
    public const float AttackRate = 0.15f;
    public const float BasicAttackLimitReached = 1f;
    public float BasicAttackLimitReachedElasped;

    public CommonCharactersData(int maxAttackPhase)
    {
        BasicAttackPhase = 0;
        BasicAttackLimitReachedElasped = 0f;
        MaxAttackPhase = maxAttackPhase;
    }
}
