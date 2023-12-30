using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentDisplay : MonoBehaviour
{
    [Header("Common Content")]
    [SerializeField] TextMeshProUGUI ItemNameText;
    [SerializeField] TextMeshProUGUI ItemTypeText;    
    [SerializeField] TextMeshProUGUI ItemDescText;
    [Header("Upgradable Content")]
    [SerializeField] GameObject LevelBackground;
    [SerializeField] TextMeshProUGUI UpgradableLevelText;
    [Header("Artifacts Content")]
    [SerializeField] TextMeshProUGUI ArtifactSetText;
    [SerializeField] TextMeshProUGUI Artifact2PieceText;
    [SerializeField] TextMeshProUGUI Artifact4PieceText;
    [SerializeField] GameObject[] ArtifactsStatsContainer;
    [SerializeField] GameObject ArtifactSetInfo;

    [Header("Common Item Info Content")]
    [SerializeField] Transform StarsTransformParent;


    void ShowArtifactsItemContent(UpgradableItems UpgradableItems, ItemTemplate itemsSO)
    {
        Artifacts artifacts = UpgradableItems as Artifacts;
        ArtifactsSO artifactsSO = itemsSO as ArtifactsSO;
        ArtifactsInfo artifactsInfo = ArtifactsManager.GetInstance().GetArtifactsInfo(artifactsSO);

        for (int i = 0; i < ArtifactsStatsContainer.Length; i++)
        {
            DisplayArtifactStats stats = ArtifactsStatsContainer[i].GetComponent<DisplayArtifactStats>();
            if (stats != null)
            {
                if (artifacts != null)
                {
                    if (i <= artifacts.GetTotalSubstatsDisplay())
                    {
                        stats.DisplayArtifactsStat(artifacts.GetArtifactStatsName(i), artifacts.GetStats(i), artifacts.GetArtifactStatsValue(i));
                    }
                    stats.gameObject.SetActive(i <= artifacts.GetTotalSubstatsDisplay());
                }
                else
                {
                    stats.gameObject.SetActive(false);
                }
            }
        }

        ArtifactSetInfo.SetActive(artifactsSO != null && artifactsInfo != null);

        if (artifactsSO == null || artifactsInfo == null)
            return;

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
    }

    private void ShowUpgradableItemContent(UpgradableItems UpgradableItems)
    {
        if (UpgradableLevelText)
        {
            LevelBackground.SetActive(UpgradableItems != null);
            if (UpgradableItems != null)
                UpgradableLevelText.text = "+" + UpgradableItems.GetLevel().ToString();
        }
    }

    public void RefreshItemContentDisplay(Item SelectedItem, ItemTemplate itemsSO)
    {
        UpgradableItems UpgradableItemREF = SelectedItem as UpgradableItems;
        ShowCommonItemContent(itemsSO);
        ShowUpgradableItemContent(UpgradableItemREF);
        ShowArtifactsItemContent(UpgradableItemREF, itemsSO);

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
