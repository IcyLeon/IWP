using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Random = UnityEngine.Random;

public class ArtifactStatsInfo
{
    private Artifacts.ArtifactsStat ArtifactsStat;
    private float StatsValue;

    public ArtifactStatsInfo(Artifacts.ArtifactsStat ArtifactsStat)
    {
        this.ArtifactsStat = ArtifactsStat;
        StatsValue = 0;
    }

    public Artifacts.ArtifactsStat GetArtifactsStat()
    {
        return ArtifactsStat;
    }

    public void SetStatsValue(float value)
    {
        StatsValue = value;
    }

    public float GetStatsValue()
    {
        return StatsValue;
    }
}

public class Artifacts : UpgradableItems
{
    private ArtifactsManager AM;
    private CharacterData characterData;
    public enum ArtifactsStat
    {
        HP,
        EM,
        DEF,
        ATK,
        HPPERCENT,
        ER,
        DEFPERCENT,
        ATKPERCENT,
        CritRate,
        CritDamage,
        TOTAL_STATS
    }

    private int TotalSubstatsDisplay;
    private List<ArtifactStatsInfo> Stats = new List<ArtifactStatsInfo>();
    private ArtifactsSet ArtifactsSet;

    public int GetTotalSubstatsDisplay()
    {
        return TotalSubstatsDisplay;
    }
    public ArtifactsSet GetArtifactsSet()
    {
        return ArtifactsSet;
    }

    public Artifacts(ArtifactsSet artifactsSet, Rarity r, ItemTemplate itemsSO, bool isNew) : base(isNew, itemsSO)
    {
        AM = ArtifactsManager.GetInstance();
        ArtifactsSet = artifactsSet;
        locked = false;
        rarity = r;

        switch (GetRarity())
        {
            case Rarity.ThreeStar:
                MaxLevel = 12;
                break;
            case Rarity.FourStar:
                MaxLevel = 16;
                break;
            case Rarity.FiveStar:
                MaxLevel = 20;
                break;
        }

        if (ArtifactsManager.isInProbabilityRange(0.7f))
        {
            TotalSubstatsDisplay = GetLowestNumberofStats(rarity).LowestDropValue;
        }
        else
        {
            TotalSubstatsDisplay = GetLowestNumberofStats(rarity).MaxDropValue;
        }
    }

    private ArtifactWeightManagement.PossibleNumberofStats GetLowestNumberofStats(Rarity rarity)
    {
        return AM.GetArtifactsListInfo().ArtifactWeightManagement.GetPossibleNumberofStatst(rarity);
    }

    private void GenerateRandomArtifacts(ArtifactsStat[] excludeArtifactsStatsList = null)
    {
        var random = new System.Random();
        int randomIndex;
        ArtifactsStat currentArtifactsStatsSelection;
        do
        {
            randomIndex = random.Next(0, (int)ArtifactsStat.TOTAL_STATS);
            currentArtifactsStatsSelection = (ArtifactsStat)randomIndex;
        } while (CheckIfStatsAlreadyExist(currentArtifactsStatsSelection, excludeArtifactsStatsList));

        ArtifactStatsInfo a = new ArtifactStatsInfo(currentArtifactsStatsSelection);
        Stats.Add(a);
    }

    private bool CheckIfStatsAlreadyExist(ArtifactsStat currentStat, ArtifactsStat[] excludeArtifactsStatsList = null)
    {
        if (excludeArtifactsStatsList != null)
        {
            foreach (var excludeStat in excludeArtifactsStatsList)
            {
                if (excludeStat == currentStat)
                {
                    return false;
                }
            }
        }

        for(int i = 0; i < Stats.Count; i++)
        {
            if (Stats[i].GetArtifactsStat() == currentStat)
                return true;
        }

        return false;
    }

    public void GenerateSubArtifactStats()
    {
        for (int i = 0; i < TotalSubstatsDisplay; i++)
        {
            GenerateRandomArtifacts();
        }
    }

    public ArtifactType GetArtifactType()
    {
        ArtifactsSO artifactsSO = ItemsSO as ArtifactsSO;
        if (artifactsSO == null)
            return default(ArtifactType);
        return artifactsSO.artifactType;
    }

    public void GenerateMainArtifactStats()
    {
        int randomIndex = Random.Range(0, AM.GetArtifactsListInfo().ArtifactWeightManagement.GetArtifactMainStatsInfoList(GetArtifactType()).Length);
        ArtifactsStat currentArtifactsStatsSelection = AM.GetArtifactsListInfo().ArtifactWeightManagement.GetArtifactMainStatsInfoList(GetArtifactType())[randomIndex].ArtifactsStat;

        ArtifactStatsInfo a = new ArtifactStatsInfo(currentArtifactsStatsSelection);
        a.SetStatsValue(GetArtifactStartingValue(a.GetArtifactsStat()));
        Stats.Add(a);
    }

    public void GenerateArtifactStats()
    {
        GenerateMainArtifactStats();
        GenerateSubArtifactStats();
    }

    public override void Upgrade()
    {
        base.Upgrade();

        UpgradeMainStats();
        if (Level % 4 == 0)
        {
            if (GetTotalSubstatsDisplay() != GetLowestNumberofStats(rarity).MaxDropValue)
            {
                GenerateRandomArtifacts();
                TotalSubstatsDisplay++;
            }
            else
            {

            }
        }
        CallOnLevelChanged();
    }

    public CharacterData GetCharacterEquipped()
    {
        return characterData;
    }

    public void SetEquippedCharacter(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public ArtifactStatsInfo GetStats(int idx)
    {
        return Stats[idx];
    }

    public string GetArtifactStatsName(int idx)
    {
        return CharacterStatsDisplay.GetStatsName(GetStats(idx).GetArtifactsStat());
    }

    private float GetArtifactStartingValue(ArtifactsStat artifactsStat)
    {
        return AM.GetArtifactsListInfo().ArtifactWeightManagement.GetArtifactStartingValue(GetArtifactType(), artifactsStat, GetRarity());
    }


    private float GetArtifactMainStatsIncreaseRatioValue(int idx)
    {
        return AM.GetArtifactsListInfo().ArtifactWeightManagement.GetArtifactIncreaseValue(GetArtifactType(), GetStats(idx).GetArtifactsStat(), GetRarity());
    }

    public float GetNextMainStatsValue(int levelIncrease)
    {
        if (Stats.Count == 0)
            return 0f;

        return GetArtifactMainStatsIncreaseRatioValue(0) * levelIncrease + GetStats(0).GetStatsValue();
    }


    private void UpgradeMainStats()
    {
        if (Stats.Count == 0)
            return;

        GetStats(0).SetStatsValue(GetStats(0).GetStatsValue() + GetArtifactMainStatsIncreaseRatioValue(0));
    }
}
