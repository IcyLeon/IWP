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
    private const int StatsActionConst = 4;
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

    public List<KeyValuePair<ArtifactsInfo, int>> GetStackAll2PieceOr4PieceList(CharacterData characterData)
    {
        Dictionary<ArtifactsInfo, int> piecesSet = GetAllArtifactsSetInCharactersInventory(characterData);

        List<KeyValuePair<ArtifactsInfo, int>> resultList = new();

        foreach (var kvp in piecesSet)
        {
            if (kvp.Value == 2)
            {
                resultList.Add(kvp);
            }
        }

        if (resultList.Count == 0)
        {
            foreach (var kvp in piecesSet)
            {
                if (kvp.Value == 4)
                {
                    resultList.Add(kvp);
                }
            }
        }

        return resultList;
    }

    public int GetTotalDuplicatePieceCount(CharacterData characterData, ArtifactsInfo artifactsInfo)
    {
        Dictionary<ArtifactsInfo, int> AmountOfpiecesSetEquipped = GetAllArtifactsSetInCharactersInventory(characterData);

        int counter = 0;

        foreach (var kvp in AmountOfpiecesSetEquipped)
        {
            if (kvp.Key == artifactsInfo)
            {
                int value = kvp.Value;
                for (int i = 0; i <= value; i++)
                {
                    if (i % 2 == 0 && i != 0)
                    {
                        counter++;
                    }
                }
            }
        }

        return counter;
    }


    private Dictionary<ArtifactsInfo, int> GetAllArtifactsSetInCharactersInventory(CharacterData characterData)
    {
        Dictionary<ArtifactsInfo, int> artifactCounts = new();

        foreach (var artifactValue in characterData.GetEquippedArtifactsList())
        {
            ArtifactsInfo ArtifactsInfo = GetArtifactsInfo(artifactValue.GetArtifactsSet());

            if (artifactCounts.ContainsKey(ArtifactsInfo))
            {
                artifactCounts[ArtifactsInfo]++;
            }
            else
            {
                artifactCounts.Add(ArtifactsInfo, 1);
            }
        }

        return artifactCounts;
    }




    public static ArtifactsManager GetInstance()
    {
        return instance;
    }
    private void Update()
    {
        //Debug only
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    Artifacts artifacts = CreateArtifact(ArtifactType.FLOWER, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.FiveStar);
        //    Artifacts artifacts2 = CreateArtifact(ArtifactType.PLUME, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.FiveStar);
        //    Artifacts artifacts3 = CreateArtifact(ArtifactType.FLOWER, ArtifactsSet.THUNDERING_FURY, Rarity.FiveStar);
        //    Artifacts artifacts4 = CreateArtifact(ArtifactType.PLUME, ArtifactsSet.THUNDERING_FURY, Rarity.FiveStar);
        //    Artifacts artifacts5 = CreateArtifact(ArtifactType.SANDS, ArtifactsSet.THUNDERING_FURY, Rarity.FiveStar);
        //    Artifacts artifacts6 = CreateArtifact(ArtifactType.GOBLET, ArtifactsSet.THUNDERING_FURY, Rarity.FiveStar);
        //    Artifacts artifacts7 = CreateArtifact(ArtifactType.SANDS, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.FourStar);
        //    Artifacts artifacts8 = CreateArtifact(ArtifactType.GOBLET, ArtifactsSet.NOBLESSE_OBLIGE, Rarity.ThreeStar);

        //    InventoryManager.GetInstance().AddItems(artifacts);
        //    InventoryManager.GetInstance().AddItems(artifacts2);
        //    InventoryManager.GetInstance().AddItems(artifacts3);
        //    InventoryManager.GetInstance().AddItems(artifacts4);
        //    InventoryManager.GetInstance().AddItems(artifacts5);
        //    InventoryManager.GetInstance().AddItems(artifacts6);
        //    InventoryManager.GetInstance().AddItems(artifacts7);
        //    InventoryManager.GetInstance().AddItems(artifacts8);
        //    InventoryManager.GetInstance().AddCurrency(CurrencyType.CASH, 999);
        //    Item item = InventoryManager.CreateItem(expItemSO);
        //    InventoryManager.GetInstance().AddItems(item);
        //}

    }

    public static float GetTotalArtifactValueStatsIncludePercentageAndBaseStats(CharacterData characterData, Artifacts.ArtifactsStat artifactsStat)
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

    public static float GetTotalFoodAndArtifactValueStatsIncludePercentageAndBaseStats(CharacterData characterData, Artifacts.ArtifactsStat artifactsStat)
    {
        if (characterData == null)
            return 0f;

        switch (artifactsStat)
        {
            case Artifacts.ArtifactsStat.HPPERCENT:
            case Artifacts.ArtifactsStat.HP:
                return (characterData.GetBaseATK(characterData.GetLevel()) * (GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.HPPERCENT) + GetTotalFoodValueStats(characterData, Artifacts.ArtifactsStat.HPPERCENT)) * 0.01f) + GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.HP) + GetTotalFoodValueStats(characterData, Artifacts.ArtifactsStat.HP);
            case Artifacts.ArtifactsStat.DEFPERCENT:
            case Artifacts.ArtifactsStat.DEF:
                return (characterData.GetBaseDEF(characterData.GetLevel()) * (GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.DEFPERCENT) + GetTotalFoodValueStats(characterData, Artifacts.ArtifactsStat.DEFPERCENT)) * 0.01f) + GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.DEF) + GetTotalFoodValueStats(characterData, Artifacts.ArtifactsStat.DEF);
            case Artifacts.ArtifactsStat.ATKPERCENT:
            case Artifacts.ArtifactsStat.ATK:
                return (characterData.GetBaseATK(characterData.GetLevel()) * (GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.ATKPERCENT) + GetTotalFoodValueStats(characterData, Artifacts.ArtifactsStat.ATKPERCENT)) * 0.01f) + GetTotalArtifactValueStats(characterData, Artifacts.ArtifactsStat.ATK) + GetTotalFoodValueStats(characterData, Artifacts.ArtifactsStat.ATK);
        }

        return GetTotalArtifactValueStatsIncludePercentageAndBaseStats(characterData, artifactsStat) + GetTotalFoodValueStats(characterData, artifactsStat);
    }

    public static float GetTotalArtifactValueStats(CharacterData characterData, Artifacts.ArtifactsStat artifactsStat)
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

    public static float GetTotalFoodValueStats(CharacterData characterData, Artifacts.ArtifactsStat foodStat)
    {
        float total = 0f;
        InventoryManager IM = InventoryManager.GetInstance();

        if (IM == null || characterData == null)
            return total;

        for (int i = 0; i < characterData.GetFoodBuffList().Count; i++)
        {
            Food food = characterData.GetFoodBuffList()[i];
            if (food != null)
            {
                BuffFoodSO buffFoodSO = food.GetFoodData() as BuffFoodSO;
                if (buffFoodSO != null)
                {
                    for (int j = 0; j < buffFoodSO.StatsBoostInfoList.Length; j++)
                    {
                        if (buffFoodSO.StatsBoostInfoList[j].StatsBoost == foodStat)
                        {
                            total += buffFoodSO.StatsBoostInfoList[j].boostValue;
                        }
                    }
                }
            }
        }

        return total;
    }

    public Artifacts CreateArtifact(ArtifactType type, ArtifactsSet artifactSet, Rarity rarity)
    {
        ArtifactsInfo artifactsinfo = GetArtifactsInfo(artifactSet);

        if (artifactsinfo != null)
        {
            ArtifactsSO artifactsSO = GetArtifactSO(type, artifactsinfo);
            Type ItemType = artifactsSO.GetTypeREF();
            object instance = Activator.CreateInstance(ItemType, artifactSet, rarity, artifactsSO, true);
            Artifacts artifacts = (Artifacts)instance;
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
