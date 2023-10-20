using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentDisplay : MonoBehaviour
{
    [Header("Artifacts Content")]
    [SerializeField] TextMeshProUGUI ItemNameText;
    [SerializeField] TextMeshProUGUI ItemTypeText;
    [SerializeField] TextMeshProUGUI ArtifactSetText;
    [SerializeField] TextMeshProUGUI ArtifactLevelText;
    [SerializeField] TextMeshProUGUI ArtifactDescText;
    [SerializeField] TextMeshProUGUI Artifact2PieceText;
    [SerializeField] TextMeshProUGUI Artifact4PieceText;
    [SerializeField] GameObject[] ArtifactsStatsContainer;

    [Header("Common Item Info Content")]
    [SerializeField] Transform StarsTransformParent;


    void ShowArtifactsItemContent(UpgradableItems UpgradableItems, ItemTemplate itemsSO)
    {
        Artifacts artifacts = UpgradableItems as Artifacts;
        ArtifactsSO artifactsSO = itemsSO as ArtifactsSO;
        ArtifactsInfo artifactsInfo = ArtifactsManager.instance.GetArtifactsInfo(artifactsSO);

        if (artifactsSO == null || artifactsInfo == null)
            return;

        if (ItemNameText)
            ItemNameText.text = artifactsSO.ItemName;
        if (ArtifactDescText)
            ArtifactDescText.text = artifactsSO.ItemDesc;
        if (ItemTypeText)
            ItemTypeText.text = artifactsSO.GetItemType();
        if (ArtifactSetText)
            ArtifactSetText.text = artifactsInfo.ArtifactsSetName + ":";
        if (Artifact2PieceText)
            Artifact2PieceText.text = "2-Piece Set: " + artifactsInfo.TwoPieceDesc;
        if (Artifact4PieceText)
            Artifact4PieceText.text = "4-Piece Set: " + artifactsInfo.FourPieceDesc;

        for (int i = 0; i < ArtifactsStatsContainer.Length; i++)
        {
            DisplayArtifactStats stats = ArtifactsStatsContainer[i].GetComponent<DisplayArtifactStats>();
            if (stats != null)
            {
                if (artifacts != null)
                {
                    if (i <= (int)artifacts.GetRarity())
                    {
                        stats.DisplayArtifactsStat(artifacts.GetArtifactStatsName(i), artifacts.GetStats(i), artifacts.GetArtifactStatsValue(i));
                    }
                    stats.gameObject.SetActive(i <= (int)artifacts.GetRarity());
                }
                else
                {
                    stats.gameObject.SetActive(false);
                }
            }
        }
        if (ArtifactLevelText)
        {
            ArtifactLevelText.gameObject.SetActive(artifacts != null);
            if (artifacts != null)
                ArtifactLevelText.text = "+" + artifacts.GetLevel().ToString();
        }
    }
    public void RefreshItemContentDisplay(Item SelectedItem, ItemTemplate itemsSO)
    {
        UpgradableItems UpgradableItemREF = SelectedItem as UpgradableItems;
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
