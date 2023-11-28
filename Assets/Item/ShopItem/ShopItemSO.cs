using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public ItemTemplate ItemsSO;
    public int Price;
    public CurrencyType CurrencyType;
}
