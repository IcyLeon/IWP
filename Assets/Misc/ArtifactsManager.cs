using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ArtifactsManager : MonoBehaviour
{
    private static ArtifactsManager instance;
    private static int StatsActionConst = 4;
    [SerializeField] ExpItemSO expItemSO; // test
    [SerializeField] ArtifactsListInfo artifactsListInfo;

    public static int GetStatsActionConst()
    {
        return StatsActionConst;
    }
    public ArtifactsListInfo GetArtifactsListInfo()
    {
        return artifactsListInfo;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static ArtifactsManager GetInstance()
    {
        return instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AddArtifactsToInventory(ArtifactType.FLOWER, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.FiveStar);
            AddArtifactsToInventory(ArtifactType.CIRCLET, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactType.GOBLET, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactType.FLOWER, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactType.PLUME, ArtifactsSet.THUNDERING_FURY, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactType.FLOWER, ArtifactsSet.THUNDERING_FURY, Rarity.FourStar);
            AddArtifactsToInventory(ArtifactType.SANDS, ArtifactsSet.THUNDERING_FURY, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactType.GOBLET, ArtifactsSet.THUNDERING_FURY, Rarity.ThreeStar);



            Type ItemType = expItemSO.GetTypeREF();
            object instance = Activator.CreateInstance(ItemType, true, expItemSO);
            ConsumableItemForbiddenInInventory ExpItem = (ConsumableItemForbiddenInInventory)instance;
            InventoryManager.GetInstance().AddItems(ExpItem);
        }

    }

    public float GetTotalArtifactValueStatsIncludePercentageAndBaseStats(CharacterData characterData, Artifacts.ArtifactsStat artifactsStat)
    {
        if (characterData == null)
            return 0f;

        switch (artifactsStat)
        {
            case Artifacts.ArtifactsStat.HPPERCENT:
            case Artifacts.ArtifactsStat.HP:
                return (characterData.GetBaseATK(characterData.GetLevel()) * GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.HPPERCENT) * 0.01f) + GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.HP);
            case Artifacts.ArtifactsStat.DEFPERCENT:
            case Artifacts.ArtifactsStat.DEF:
                return (characterData.GetBaseDEF(characterData.GetLevel()) * GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.DEFPERCENT) * 0.01f) + GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.DEF);
            case Artifacts.ArtifactsStat.ATKPERCENT:
            case Artifacts.ArtifactsStat.ATK:
                return (characterData.GetBaseATK(characterData.GetLevel()) * GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.ATKPERCENT) * 0.01f) + GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.ATK);
        }

        return GetTotalArtifactValueStats(characterData, artifactsStat);
    }
    public float GetTotalArtifactValueStats(CharacterData characterData, Artifacts.ArtifactsStat artifactsStat)
    {
        float total = 0f;
        InventoryManager IM = InventoryManager.GetInstance();

        if (IM == null || characterData == null)
            return total;

        for (int i = 0; i < characterData.GetEquippedArtifactsList().Count; i++)
        {
            Artifacts artifacts = characterData.GetEquippedArtifactsList()[i];
            if (artifacts != null)
            {
                for (int j = 0; j < artifacts.GetTotalStats(); j++)
                {
                    if (artifacts.GetStats(j).GetArtifactsStat() == artifactsStat)
                    {
                        total += artifacts.GetStats(j).GetStatsValue();
                    }
                }
            }
        }

        return total;
    }

    public Artifacts AddArtifactsToInventory(ArtifactType type, ArtifactsSet artifactSet, Rarity rarity)
    {
        ArtifactsInfo artifactsinfo = GetArtifactsInfo(artifactSet);

        if (artifactsinfo != null)
        {
            ArtifactsSO artifactsSO = GetArtifactSO(type, artifactsinfo);
            Type ItemType = artifactsSO.GetTypeREF();
            object instance = Activator.CreateInstance(ItemType, artifactSet, rarity, artifactsSO, true);
            Artifacts artifacts = (Artifacts)instance;

            artifacts.GenerateArtifactStats();
            InventoryManager.GetInstance().AddItems(artifacts);
            return artifacts;
        }
        return null;
    }

    public ArtifactsInfo GetArtifactsInfo(ArtifactsSet artifactsSet)
    {
        for (int i = 0; i < artifactsListInfo.artifactsInfo.Length; i++)
        {
            if (artifactsListInfo.artifactsInfo[i].ArtifactSet == artifactsSet)
            {
                return artifactsListInfo.artifactsInfo[i];
            }
        }
        return null;
    }

    public ArtifactsInfo GetArtifactsInfo(ArtifactsSO artifactsSO)
    {
        for (int i = 0; i < artifactsListInfo.artifactsInfo.Length; i++)
        {
            for (int j = 0; j < artifactsListInfo.artifactsInfo[i].artifactSOList.Length; j++) {
                if (artifactsListInfo.artifactsInfo[i].artifactSOList[j] == artifactsSO)
                {
                    return artifactsListInfo.artifactsInfo[i];
                }
            }
        }
        return null;
    }



    public ArtifactsSO GetArtifactSO(ArtifactType ArtifactType, ArtifactsInfo artifactsInfo)
    {
        for (int i = 0; i < artifactsListInfo.artifactsInfo.Length; i++)
        {
            for (int j = 0; j < artifactsListInfo.artifactsInfo[i].artifactSOList.Length; j++)
            {
                if (artifactsInfo == artifactsListInfo.artifactsInfo[i])
                {
                    if (ArtifactType == artifactsListInfo.artifactsInfo[i].artifactSOList[j].artifactType)
                    {
                        return artifactsListInfo.artifactsInfo[i].artifactSOList[j];
                    }
                }
            }
        }
        return null;
    }



    public ArtifactsListInfo.CommonArtifactPiece GetArtifactPiece(ArtifactType ArtifactType)
    {
        for (int j = 0; j < artifactsListInfo.artifactsInfoTypeName.Length; j++)
        {
            if (ArtifactType == artifactsListInfo.artifactsInfoTypeName[j].artifactType)
            {
                return artifactsListInfo.artifactsInfoTypeName[j];
            }
        }
        return null;
    }
}
