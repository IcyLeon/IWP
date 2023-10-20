using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Artifacts : UpgradableItems
{
    private Characters characters;
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
        DMGBONUS,
        CritRate,
        CritDamage
    }

    private Dictionary<ArtifactsStat, string> ArtifactStatsName = new()
    {
        { ArtifactsStat.HP, "HP" },
        { ArtifactsStat.HPPERCENT, "HP" },
        { ArtifactsStat.EM, "Elemental Mastery" },
        { ArtifactsStat.DEF, "DEF" },
        { ArtifactsStat.DEFPERCENT, "DEF" },
        { ArtifactsStat.ER, "Energy Recharge" },
        { ArtifactsStat.ATK, "ATK" },
        { ArtifactsStat.ATKPERCENT, "ATK" },
        { ArtifactsStat.CritRate, "CRIT Rate" },
        { ArtifactsStat.CritDamage, "CRIT DMG" },
        { ArtifactsStat.DMGBONUS, "DMG BONUS" }
    };

    public ArtifactsStat[] StatsList = {
        ArtifactsStat.HP,
        ArtifactsStat.EM,
        ArtifactsStat.DEF,
        ArtifactsStat.ER,
        ArtifactsStat.ATK,
        ArtifactsStat.HPPERCENT,
        ArtifactsStat.DEFPERCENT,
        ArtifactsStat.ATKPERCENT,
        ArtifactsStat.DMGBONUS,
        ArtifactsStat.CritRate,
        ArtifactsStat.CritDamage
    };
    private List<ArtifactsStat> Stats = new List<ArtifactsStat>();

    private Dictionary<ArtifactsStat, Dictionary<Rarity, float>> MainStatsInfo = new()
    {  
        {
            ArtifactsStat.HP, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 430f  },
                { Rarity.FourStar, 645f },
                { Rarity.FiveStar, 717f }
            }
        },
        {
            ArtifactsStat.HPPERCENT, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 5.2f },
                { Rarity.FourStar, 6.3f },
                { Rarity.FiveStar, 7.0f }
            }
        },
        {
            ArtifactsStat.ATK, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 28f },
                { Rarity.FourStar, 42f },
                { Rarity.FiveStar, 47f }
            }
        },
        {
            ArtifactsStat.ATKPERCENT, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 5.2f },
                { Rarity.FourStar, 6.3f },
                { Rarity.FiveStar, 7.0f }
            }
        },
        {
            ArtifactsStat.DEFPERCENT, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 6.6f },
                { Rarity.FourStar, 7.9f },
                { Rarity.FiveStar, 8.7f }
            }
        },
        {
            ArtifactsStat.EM, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 21f },
                { Rarity.FourStar, 25f },
                { Rarity.FiveStar, 28f }
            }
        },
        {
            ArtifactsStat.ER, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 5.8f },
                { Rarity.FourStar, 7.0f },
                { Rarity.FiveStar, 7.8f }
            }
        },
        {
            ArtifactsStat.DMGBONUS, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 5.2f },
                { Rarity.FourStar, 6.3f },
                { Rarity.FiveStar, 7.0f }
            }
        },
        {
            ArtifactsStat.CritDamage, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 7.0f },
                { Rarity.FourStar, 8.4f },
                { Rarity.FiveStar, 9.3f }
            }
        },
        {
            ArtifactsStat.CritRate, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 3.5f },
                { Rarity.FourStar, 4.2f },
                { Rarity.FiveStar, 4.7f }
            }
        },

        {
            ArtifactsStat.DEF, new Dictionary<Rarity, float>() {
                { Rarity.ThreeStar, 3.5f },
                { Rarity.FourStar, 4.2f },
                { Rarity.FiveStar, 4.7f }
            }
        }
    };

    private ArtifactsSet ArtifactsSet;

    public ArtifactsSet GetArtifactsSet()
    {
        return ArtifactsSet;
    }

    public Artifacts(ArtifactType type, ArtifactsSet artifactsSet, Rarity rarity)
    {
        ArtifactsSet = artifactsSet;
        ArtifactsInfo artifactsinfo = ArtifactsManager.instance.GetArtifactsInfo(ArtifactsSet);
        locked = false;
        ItemsSO = ArtifactsManager.instance.GetArtifactSO(type, artifactsinfo);
        category = Category.ARTIFACTS;
        this.rarity = rarity;
    }


    private void GenerateRandomArtifacts(ArtifactsStat[] excludeArtifactsStatsList = null)
    {
        var random = new System.Random();
        int randomIndex;
        ArtifactsStat currentArtifactsStatsSelection;
        do
        {
            randomIndex = random.Next(0, StatsList.Length);
            currentArtifactsStatsSelection = StatsList[randomIndex];
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

    public int GetMaxLevel()
    {
        return MaxLevel;
    }

    public void GenerateSubArtifactStats()
    {
        for (int i = 0; i < (int)GetRarity(); i++)
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
        switch (GetArtifactType())
        {
            case ArtifactType.FLOWER:
                Stats.Add(ArtifactsStat.HP);
                break;
            case ArtifactType.PLUME:
                Stats.Add(ArtifactsStat.ATK);
                break;
            case ArtifactType.SANDS:
                {
                    ArtifactsStat[] excludeArtifactsStatsList = { ArtifactsStat.DMGBONUS, ArtifactsStat.CritRate, ArtifactsStat.CritDamage, ArtifactsStat.DEF, ArtifactsStat.HP, ArtifactsStat.ATK };
                    GenerateRandomArtifacts(excludeArtifactsStatsList);
                }
                break;
            case ArtifactType.GOBLET:
                {
                    ArtifactsStat[] excludeArtifactsStatsList = { ArtifactsStat.DEF, ArtifactsStat.HP, ArtifactsStat.ATK, ArtifactsStat.ER };
                    GenerateRandomArtifacts(excludeArtifactsStatsList);
                }
                break;
            case ArtifactType.CIRCLET:
                {
                    ArtifactsStat[] excludeArtifactsStatsList = { ArtifactsStat.DMGBONUS, ArtifactsStat.DEF, ArtifactsStat.HP, ArtifactsStat.ATK, ArtifactsStat.ER };
                    GenerateRandomArtifacts(excludeArtifactsStatsList);
                }
                break;
        }
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
        if (Level % 4 == 0)
        {

        }
        base.Upgrade();
    }

    public Characters GetCharacter()
    {
        return characters;
    }

    public void SetEquippedCharacter(Characters characters)
    {
        this.characters = characters;
    }

    public ArtifactsStat GetStats(int idx)
    {
        return Stats[idx];
    }

    public string GetArtifactStatsName(int idx)
    {
        return ArtifactStatsName[GetStats(idx)];
    }

    public string GetArtifactStatsValue(int idx)
    {
        return MainStatsInfo[GetStats(idx)][GetRarity()].ToString();
    }
}
