using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    private float GetStatsPossibleRoll(float highestRoll, int size, float diff)
    {
        float[] PossibleRolls = new float[size];

        for (int i = 0; i < PossibleRolls.Length; i++)
        {
            PossibleRolls[i] = highestRoll * (1.0f - (i * diff));
        }

        var randomIdx = Random.Range(0, PossibleRolls.Length);
        return PossibleRolls[randomIdx];
    }

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

        if (AssetManager.isInProbabilityRange(0.66f))
        {
            TotalSubstatsDisplay = GetLowestNumberofStats(rarity).LowestDropValue;
        }
        else
        {
            TotalSubstatsDisplay = GetLowestNumberofStats(rarity).MaxDropValue;
        }

        GenerateArtifactStats(); // main stats
    }

    public ArtifactWeightManagement.PossibleNumberofStats GetLowestNumberofStats(Rarity rarity)
    {
        return AM.GetArtifactsListInfo().ArtifactWeightManagement.GetPossibleNumberofStats(rarity);
    }

    private void GenerateRandomArtifacts()
    {
        ArtifactsStat currentArtifactsStatsSelection;
        do
        {
            int randomIndex = Random.Range(0, (int)ArtifactsStat.TOTAL_STATS);
            currentArtifactsStatsSelection = (ArtifactsStat)randomIndex;
        } while (CheckIfStatsAlreadyExist(currentArtifactsStatsSelection));

        ArtifactStatsInfo a = new ArtifactStatsInfo(currentArtifactsStatsSelection);
        float Value = GetStatsPossibleRoll(GetHighestPossibleStatsRoll(a.GetArtifactsStat()), 4, 0.1f);
        a.SetStatsValue(Value);
        Stats.Add(a);
    }

    private bool CheckIfStatsAlreadyExist(ArtifactsStat currentStat)
    {
        for(int i = 0; i < GetTotalStats(); i++)
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
        if (GetArtifactsSO() == null)
            return default(ArtifactType);
        return GetArtifactsSO().artifactType;
    }

    public ArtifactsSO GetArtifactsSO()
    {
        return ItemsSO as ArtifactsSO;
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
        if (Level % ArtifactsManager.GetStatsActionConst() == 0)
        {
            if (GetTotalSubstatsDisplay() != GetLowestNumberofStats(rarity).MaxDropValue)
            {
                GenerateRandomArtifacts();
                TotalSubstatsDisplay++;
            }
            else
            {
                int randomStat = Random.Range(1, GetTotalStats());
                ArtifactStatsInfo a = GetStats(randomStat);
                float Value = GetStatsPossibleRoll(GetHighestPossibleStatsRoll(a.GetArtifactsStat()), 4, 0.1f);
                a.SetStatsValue(a.GetStatsValue() + Value);
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

    public int GetTotalStats()
    {
        return Stats.Count;
    }

    public string GetArtifactStatsName(int idx)
    {
        return CharacterStatsDisplay.GetStatsName(GetStats(idx).GetArtifactsStat());
    }

    private float GetArtifactStartingValue(ArtifactsStat artifactsStat)
    {
        return AM.GetArtifactsListInfo().ArtifactWeightManagement.GetArtifactStatsValueInfo(GetArtifactType(), artifactsStat, GetRarity()).Evaluate(0f);
    }
    private float GetHighestPossibleStatsRoll(ArtifactsStat artifactsStat)
    {
        return AM.GetArtifactsListInfo().ArtifactWeightManagement.GetHighestPossibleStatsRoll(artifactsStat, GetRarity());
    }

    private float GetArtifactMainStatsValueInfo(int idx, int level)
    {
        ArtifactsStat a = GetStats(idx).GetArtifactsStat();
        float value = AM.GetArtifactsListInfo().ArtifactWeightManagement.GetArtifactStatsValueInfo(GetArtifactType(), GetStats(idx).GetArtifactsStat(), GetRarity()).Evaluate(level);

        if (ArtifactsManager.CheckIfInBetweenStats_PERCENT(a))
        {
            value = Mathf.Round(value * 10f) / 10f;
        }
        else
        {
            value = Mathf.Round(value);
        }

        return value;
    }

    public float GetNextMainStatsValue(int levelIncrease)
    {
        if (GetTotalStats() == 0)
            return 0f;

        return GetArtifactMainStatsValueInfo(0, GetLevel() + levelIncrease);
    }


    private void UpgradeMainStats()
    {
        if (GetTotalStats() == 0)
            return;

        GetStats(0).SetStatsValue(GetArtifactMainStatsValueInfo(0, GetLevel()));
    }
}
