using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Category
{
    FOOD,
    COLLECTIBLES,
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

    public virtual Type GetTypeREF()
    {
        return typeof(Item);
    }

    public virtual Category GetCategory()
    {
        return default(Category);
    }

    public virtual string GetItemType()
    {
        return "Unknown Type";
    }
}
