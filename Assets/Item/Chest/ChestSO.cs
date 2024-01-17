using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ChestSO")]
public class ChestSO : PurchaseableObjectSO
{
    public ScriptableObject[] ItemDropsList;
    public int MaxTotalDrops;
    public int BaseCoinsDrops;
    public int BaseCashsDrops;
}
