using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Artifacts;

[CreateAssetMenu(fileName = "ArtifactWeightManagementSO", menuName = "ScriptableObjects/ArtifactWeightManagementSO")]
public class ArtifactWeightManagement : ScriptableObject
{
    [Serializable]
    public class ArtifactWeight
    {

        public int Weight;
        public ArtifactsStat ArtifactsStat;
    }

    [Serializable]
    public class StartingArtifactStatsHand
    {
        public Rarity rarity;
        public float increaseValueRatio;
        public float value;
    }

    [Serializable]
    public class ArtifactMainStatsInfo
    {
        public StartingArtifactStatsHand[] StartingArtifactStatsHand;
        public ArtifactsStat ArtifactsStat;
    }

    [Serializable]
    public class ArtifactPossibleStats
    {
        public ArtifactMainStatsInfo[] ArtifactMainStatsInfoList;
        public ArtifactType ArtifactType;
    }

    [Serializable]
    public class PossibleNumberofStats
    {
        public Rarity rarity;
        public int LowestDropValue;
        public int MaxDropValue;
    }

    public ArtifactPossibleStats[] artifactPossibleStatsList;
    public ArtifactWeight[] artifactWeightsList;
    public PossibleNumberofStats[] PossibleNumberofStatsList;

    public PossibleNumberofStats GetPossibleNumberofStatst(Rarity rarity)
    {
        for (int i = 0; i < PossibleNumberofStatsList.Length; i++)
        {
            if (PossibleNumberofStatsList[i].rarity == rarity)
            {
                return PossibleNumberofStatsList[i];
            }
        }
        return null;
    }

    public ArtifactMainStatsInfo[] GetArtifactMainStatsInfoList(ArtifactType ArtifactType)
    {
        for (int i = 0; i < artifactPossibleStatsList.Length; i++)
        {
            if (artifactPossibleStatsList[i].ArtifactType == ArtifactType)
            {
                return artifactPossibleStatsList[i].ArtifactMainStatsInfoList;
            }
        }
        return null;
    }

    public float GetArtifactIncreaseValue(ArtifactType ArtifactType, ArtifactsStat ArtifactsStat, Rarity rarity)
    {
        ArtifactMainStatsInfo[] list = GetArtifactMainStatsInfoList(ArtifactType);

        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].ArtifactsStat == ArtifactsStat)
            {
                for (int j = 0; j < list[i].StartingArtifactStatsHand.Length; j++)
                {
                    StartingArtifactStatsHand StartingArtifactStatsHand = list[i].StartingArtifactStatsHand[j];
                    if (StartingArtifactStatsHand.rarity == rarity)
                    {
                        return StartingArtifactStatsHand.increaseValueRatio;
                    }
                }
            }
        }

        return 0f;
    }

    public float GetArtifactStartingValue(ArtifactType ArtifactType, ArtifactsStat ArtifactsStat, Rarity rarity)
    {
        ArtifactMainStatsInfo[] list = GetArtifactMainStatsInfoList(ArtifactType);

        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].ArtifactsStat == ArtifactsStat)
            {
                for (int j = 0; j < list[i].StartingArtifactStatsHand.Length; j++)
                {
                    StartingArtifactStatsHand StartingArtifactStatsHand = list[i].StartingArtifactStatsHand[j];
                    if (StartingArtifactStatsHand.rarity == rarity)
                    {
                        return StartingArtifactStatsHand.value;
                    }
                }
            }
        }

        return 0f;
    }
    public ArtifactWeight GetArtifactWeight(ArtifactsStat ArtifactsStat)
    {
        for (int i = 0; i < artifactWeightsList.Length; i++)
        {
            if (artifactWeightsList[i].ArtifactsStat == ArtifactsStat)
                return artifactWeightsList[i];
        }
        return null;
    }
}
