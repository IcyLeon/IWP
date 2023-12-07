using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FriendlyKillerSO")]
public class PurchaseableObjectSO : ScriptableObject
{
    public string PurchaseableObjectName;
    public Sprite PurchaseableObjectSprite;
    public int BasePrice;
}
