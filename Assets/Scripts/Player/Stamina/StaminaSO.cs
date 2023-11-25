using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StaminaSO")]
public class StaminaSO : ScriptableObject
{
    public float DashCost;
    public float SprintCostPerSec;
    public float RegenStaminaPerSec;
}
