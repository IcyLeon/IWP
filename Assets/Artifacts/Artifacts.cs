using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Random = UnityEngine.Random;

public class Artifacts : UpgradableItems
{
    private ArtifactsManager AM;
    private CharacterData characterData;
    public enum ArtifactsStat
    {
        HP,
        EM,
        DEF,
        ER,
        ATK,
        HPPERCENT,
        DEFPERCENT,
        ATKPERCENT,
        CritRate,
        CritDamage,
        TOTAL_STATS
    }

    private int TotalSubstatsDisplay;
    private List<ArtifactsStat> Stats = new List<ArtifactsStat>();
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


        if (ArtifactsManager.isInProbabilityRange(0.66f))
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

        Stats.Add(currentArtifactsStatsSelection);
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

        return Stats.Contains(currentStat);
    }

    public override int GetMaxLevel()
    {
        return MaxLevel;
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
        Stats.Add(currentArtifactsStatsSelection);
    }

    public void GenerateArtifactStats()
    {
        GenerateMainArtifactStats();
        GenerateSubArtifactStats();

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
    }

    public override void Upgrade()
    {
        base.Upgrade();

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
    }

    public CharacterData GetCharacterEquipped()
    {
        return characterData;
    }

    public void SetEquippedCharacter(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public ArtifactsStat GetStats(int idx)
    {
        return Stats[idx];
    }

    public string GetArtifactStatsName(int idx)
    {
        return CharacterStatsDisplay.GetStatsName(GetStats(idx));
    }

    public string GetArtifactStatsValue(int idx)
    {
        return AM.GetArtifactsListInfo().ArtifactWeightManagement.GetArtifactStartingValue(GetArtifactType(), GetStats(idx), GetRarity()).ToString();
    }
}
