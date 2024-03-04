using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowData : CommonCharactersData
{
    public bool isChargedFinish;

    public Elemental CurrentElemental;
    public float ChargedMaxElapsed = 1.5f; // do not change
    public float ChargeElapsed;

    public const float BaseFireSpeed = 15f;
    public const float ChargedAttackRate = 0.5f;
    public Elemental ShootElemental;
    public Vector3 Direction;

    public BowData(int maxAttackPhase) : base(maxAttackPhase)
    {
    }
}
