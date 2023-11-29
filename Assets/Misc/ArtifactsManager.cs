using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ArtifactsManager : MonoBehaviour
{
    private static ArtifactsManager instance;
    [SerializeField] ArtifactsListInfo artifactsListInfo;

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


        }

    }

    public Artifacts AddArtifactsToInventory(ArtifactType type, ArtifactsSet artifactSet, Rarity rarity)
    {
        ArtifactsInfo artifactsinfo = GetArtifactsInfo(artifactSet);

        if (artifactsinfo != null)
        {
            ArtifactsSO artifactsSO = GetArtifactSO(type, artifactsinfo);
            Type ItemType = artifactsSO.GetType();
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



    public string GetArtifactPieceName(ArtifactType ArtifactType)
    {
        for (int j = 0; j < artifactsListInfo.artifactsInfoTypeName.Length; j++)
        {
            if (ArtifactType == artifactsListInfo.artifactsInfoTypeName[j].artifactType)
            {
                return artifactsListInfo.artifactsInfoTypeName[j].artifactPieceName;
            }
        }
        return null;
    }
}
