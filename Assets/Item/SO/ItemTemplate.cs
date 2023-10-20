using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Category
{
    FOOD,
    ARTIFACTS
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemsSO")]
public class ItemTemplate : ScriptableObject
{
    public string ItemName;
    [TextAreaAttribute]
    public string ItemDesc;
    public Sprite ItemSprite;
    public Rarity Rarity;

    public virtual string GetItemType()
    {
        return "Unknown Type";
    }
}
