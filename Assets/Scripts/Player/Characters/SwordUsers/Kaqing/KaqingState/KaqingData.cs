using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingData : CommonCharactersData
{
    public float ESkillRange = 6f;
    public KaqingTeleporter kaqingTeleporter;

    public KaqingData(int maxAttackPhase) : base(maxAttackPhase)
    {
    }
}
