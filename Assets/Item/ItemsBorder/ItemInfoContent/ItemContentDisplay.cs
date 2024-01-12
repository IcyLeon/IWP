using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentDisplay : MonoBehaviour
{
    [Header("Common Content")]
    [SerializeField] TextMeshProUGUI ItemNameText;
    [SerializeField] TextMeshProUGUI ItemTypeText;    
    [SerializeField] TextMeshProUGUI ItemDescText;
    [SerializeField] Image ItemIconTypeImage;

    [Header("Upgradable Content")]
    [SerializeField] TextMeshProUGUI UpgradableLevelText;
    [SerializeField] GameObject UpgradableContent;

    [Header("Artifacts Content")]
    [SerializeField] TextMeshProUGUI ArtifactSetText;
    [SerializeField] TextMeshProUGUI Artifact2PieceText;
    [SerializeField] TextMeshProUGUI Artifact4PieceText;
    [SerializeField] GameObject ArtifactsStatsContainer;
    [SerializeField] GameObject ArtifactsContent;
    [SerializeField] GameObject ArtifactsStatsContent;
    [SerializeField] GameObject ArtifactsSets;

    [Header("Food Info Content")]
    [SerializeField] GameObject FoodsContent;

    [Header("Common Item Info Content")]
    [SerializeField] Transform StarsTransformParent;

    void ShowArtifactsItemContent(UpgradableItems UpgradableItems, ItemTemplate itemsSO)
    {
        Artifacts artifacts = UpgradableItems as Artifacts;
        ArtifactsSO artifactsSO = itemsSO as ArtifactsSO;
        ArtifactsInfo artifactsInfo = ArtifactsManager.GetInstance().GetArtifactsInfo(artifactsSO);

        ArtifactsStatsContent.SetActive(artifacts != null);
        ArtifactsSets.SetActive(artifactsSO != null && artifactsInfo != null);
        ArtifactsContent.SetActive(ArtifactsSets.activeSelf);

        if (artifactsSO == null || artifactsInfo == null)
            return;

        int i = 0;

        foreach(var stats in ArtifactsStatsContainer.GetComponentsInChildren<DisplayArtifactStats>(true))
        {
            if (stats != null)
            {
                if (artifacts != null)
                {
                    if (i <= artifacts.GetTotalSubstatsDisplay())
                    {
                        stats.DisplayArtifactsStat(artifacts.GetArtifactStatsName(i), artifacts.GetStats(i), artifacts);
                    }
                    stats.gameObject.SetActive(i <= artifacts.GetTotalSubstatsDisplay());
                }
                else
                {
                    stats.gameObject.SetActive(false);
                }
            }
            i++;
        }

        if (ArtifactSetText)
            ArtifactSetText.text = artifactsInfo.ArtifactsSetName + ":";
        if (Artifact2PieceText)
            Artifact2PieceText.text = "2-Piece Set: " + artifactsInfo.TwoPieceDesc;
        if (Artifact4PieceText)
            Artifact4PieceText.text = "4-Piece Set: " + artifactsInfo.FourPieceDesc;
    }

    private void ShowCommonItemContent(ItemTemplate itemsSO)
    {
        if (ItemNameText)
            ItemNameText.text = itemsSO.ItemName;
        if (ItemTypeText)
            ItemTypeText.text = itemsSO.GetItemType();
        if (ItemDescText)
            ItemDescText.text = itemsSO.ItemDesc;

        if (ItemIconTypeImage && itemsSO != null)
            ItemIconTypeImage.sprite = itemsSO.ItemIconTypeSprite;

        ItemIconTypeImage.gameObject.SetActive(itemsSO != null && itemsSO?.ItemIconTypeSprite != null);
    }

    private void ShowUpgradableItemContent(UpgradableItems UpgradableItems)
    {
        UpgradableContent.SetActive(UpgradableItems != null);
        if (UpgradableItems == null)
            return;

        if (UpgradableLevelText)
        {
            UpgradableLevelText.text = "+" + UpgradableItems.GetLevel().ToString();
        }

    }

    private void ShowFoodItemContent(FoodData FoodDataSO)
    {
        FoodsContent.SetActive(FoodDataSO != null);
        if (FoodDataSO == null)
            return;
        int i = 0;

        foreach (var stats in FoodsContent.GetComponentsInChildren<DisplayFoodStatsInfo>(true))
        {
            if (stats != null)
            {
                if (i < FoodDataSO.FoodStatsInfo.Length)
                {
                    stats.DisplayFoodsStat(FoodDataSO.FoodStatsInfo[i]);
                }
                stats.gameObject.SetActive(i < FoodDataSO.FoodStatsInfo.Length);
            }
            i++;
        }
    }

    public void RefreshItemContentDisplay(Item SelectedItem, ItemTemplate itemsSO)
    {
        UpgradableItems UpgradableItemREF = SelectedItem as UpgradableItems;
        ShowCommonItemContent(itemsSO);
        ShowUpgradableItemContent(UpgradableItemREF);
        ShowArtifactsItemContent(UpgradableItemREF, itemsSO);
        ShowFoodItemContent(itemsSO as FoodData);

        if (StarsTransformParent)
        {
            foreach (Transform child in StarsTransformParent)
            {
                Destroy(child.gameObject);
            }

            if (SelectedItem == null)
            {
                SpawnStars((int)itemsSO.Rarity);
            }
            else
            {
                SpawnStars((int)SelectedItem.GetRarity());
            }
        }

    }

    private void SpawnStars(int amt)
    {
        for (int i = 0; i <= amt; i++)
        {
            GameObject go = Instantiate(AssetManager.GetInstance().StarPrefab, StarsTransformParent);
        }
    }
}
