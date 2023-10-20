using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    OneStar,
    TwoStar,
    ThreeStar,
    FourStar,
    FiveStar
}

[CreateAssetMenu(fileName = "ItemsListData", menuName = "ScriptableObjects/ItemsListData")]
public class ItemsList : ScriptableObject
{
    [System.Serializable]
    public class Raritylist
    {
        public Rarity rarity;
        public Sprite rarityborderimage;
        public Sprite ItemCardImage;
    }

    public Raritylist[] raritylist;



    [System.Serializable]
    public class UpgradeableItemCostList
    {
        public Rarity rarity;
        public int[] Cost;
    }
    public UpgradeableItemCostList[] UpgradeitemCostList;
    public int[] GetCostListStatus(Rarity rarity)
    {
        for (int i = 0; i < UpgradeitemCostList.Length; i++)
        {
            if (rarity == UpgradeitemCostList[i].rarity)
            {
                return UpgradeitemCostList[i].Cost;
            }
        }
        return null;
    }
}
