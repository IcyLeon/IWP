using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine.AI;


public class EnemyManager : MonoBehaviour
{
    private List<BaseEnemy> CurrentEnemySpawnedList;
    private List<BaseEnemy> BossEnemyList = new();
    public delegate void onEnemyChanged(BaseEnemy enemy);
    public onEnemyChanged OnEnemyKilled;
    public onEnemyChanged OnBossEnemyAdd;
    public onEnemyChanged OnBossEnemyRemove;
    private SceneManager SceneManager;
    private static EnemyManager instance;
    private int CurrentWave;
    private int CurrentEnemyDefeated;
    private int TotalEnemies;
    private int TotalEnemiesDefeated;
    public static Action OnEnemyDefeatedChange;
    public static Action OnEnemyWaveChange;

    [SerializeField] EnemyInfo EnemyInfoSO;

    public static Vector3 GetRandomPointWithinTerrain(Terrain terrain)
    {
        Vector3 terrainPosition = terrain.GetPosition();
        Vector3 terrainSize = terrain.terrainData.size;
        float randomX = Random.Range(terrainPosition.x, terrainPosition.x + terrainSize.x);
        float randomZ = Random.Range(terrainPosition.z, terrainPosition.z + terrainSize.z);
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0f, randomZ));
        Vector3 randomTerrainPosition = new Vector3(randomX, terrainHeight, randomZ);

        return randomTerrainPosition;
    }
    private int GetMaxEnemyInScene()
    {
        SceneManager sm = SceneManager.GetInstance();

        switch (sm.GetCurrentScene())
        {
            case SceneEnum.BOSS:
                return 5;
            case SceneEnum.GAME:
                return 20;
        }
        return 0;
    }

    public bool HasNotReachSpawnLimit()
    {
        return GetTotalCurrentEnemySpawnedList() < GetMaxEnemyInScene();
    }

    public int GetTotalCurrentEnemySpawnedList()
    {
        return CurrentEnemySpawnedList.Count;
    }
    public int GetTotalEnemyAliveInList()
    {
        int Count = 0;
        for (int i = 0; i < GetTotalCurrentEnemySpawnedList(); i++)
        {
            BaseEnemy enemy = CurrentEnemySpawnedList[i];
            if (enemy != null)
            {
                if (!enemy.IsDead())
                    Count++;
            }
        }
        return Count;
    }

    public EnemyInfo GetEnemyInfo()
    {
        return EnemyInfoSO;
    }
    public BaseEnemy SpawnGroundUnitsWithinTerrain(CharactersSO charactersSO, Terrain terrain)
    {
        Vector3 spawnPosition = GetRandomPointWithinTerrain(terrain);

        BaseEnemy enemy = Instantiate(EnemyInfoSO.GetEnemyContent(charactersSO).EnemyPrefab, spawnPosition, Quaternion.identity).GetComponent<BaseEnemy>();
        CurrentEnemySpawnedList.Add(enemy);
        return enemy;
    }

    public void EnemyDropRandomItem(CharactersSO charactersSO, Vector3 SpawnPosition)
    {
        List<Item> itemsList = new();
        EnemyContent enemyContent = EnemyInfoSO.GetEnemyContent(charactersSO);
        int amtToDrop = Random.Range(0, enemyContent.MaxItemToGive + 1);
        int currentAmt = 0;
        ArtifactsManager AM = ArtifactsManager.GetInstance();

        do
        {
            int random = Random.Range(0, enemyContent.PossibleItemsList.Length);
            ScriptableObject item = enemyContent.PossibleItemsList[random];
            if (item == null)
                continue;

            switch (item)
            {
                case ItemTemplate itemTemplate:
                    Type ItemType = itemTemplate.GetTypeREF();
                    object instance = Activator.CreateInstance(ItemType, true, itemTemplate);
                    Item itemREF = (Item)instance;
                    itemsList.Add(itemREF);
                    break;
                case ArtifactsListInfo artifactsListInfo:
                    int randomArtifactsInfo = Random.Range(0, artifactsListInfo.artifactsInfo.Length);
                    ArtifactsInfo artifactsInfo = artifactsListInfo.artifactsInfo[randomArtifactsInfo];
                    if (artifactsInfo != null)
                    {
                        int randomArtifactsSO = Random.Range(0, artifactsInfo.artifactSOList.Length);
                        ArtifactsSO artifactsSO = artifactsInfo.artifactSOList[randomArtifactsSO];

                        Rarity randomRarity = Rarity.ThreeStar;
                        if (AssetManager.isInProbabilityRange(0.1f))
                        {
                            randomRarity = Rarity.FiveStar;
                        }
                        else
                        {
                            if (AssetManager.isInProbabilityRange(0.75f))
                            {
                                randomRarity = Rarity.ThreeStar;
                            }
                            else
                            {
                                randomRarity = Rarity.FourStar;
                            }
                        }
                        Artifacts artifact = AM.CreateArtifact(artifactsSO.artifactType, artifactsInfo.ArtifactSet, randomRarity);
                        itemsList.Add(artifact);
                    }

                    break;

                default:
                    break;
            }
            currentAmt++;

        } while (currentAmt < amtToDrop);

        SpawnItemDropGO(itemsList, SpawnPosition);
    }

    private void SpawnItemDropGO(List<Item> ItemList, Vector3 pos)
    {
        AssetManager AM = AssetManager.GetInstance();
        if (ItemList == null)
            return;

        for (int i = 0; i < ItemList.Count; i++)
        {
            Item item = ItemList[i];
            AM.SpawnItemDrop(item, pos);
        }
    }


    public void CallOnEnemyKilled(BaseEnemy enemy)
    {
        TotalEnemiesDefeated++;
        InventoryManager.GetInstance().AddCurrency(CurrencyType.CASH, Mathf.RoundToInt(enemy.GetDropValue()));
        OnEnemyKilled?.Invoke(enemy);
    }

    public List<BaseEnemy> GetBossEnemyList()
    {
        return BossEnemyList;
    }
    public void AddBossToList(BaseEnemy e)
    {
        if (e != null)
        {
            OnBossEnemyAdd?.Invoke(e);
            BossEnemyList.Add(e);
        }
    }


    public static EnemyManager GetInstance()
    {
        return instance;
    }

    public void SetEnemiesCount(int value)
    {
        TotalEnemies = value;
    }

    public int GetEnemiesCount()
    {
        return TotalEnemies;
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

        CurrentEnemySpawnedList = new();
    }

    private void Start()
    {
        SceneManager = SceneManager.GetInstance();
        ResetTotalEnemiesDefeated();
        SetCurrentWave(0);
    }

    private void Update()
    {
        UpdateRemoveEnemyList();
    }

    public void DestroyAllEnemies()
    {
        for (int i = GetTotalCurrentEnemySpawnedList() - 1; i >= 0; i--)
        {
            if (CurrentEnemySpawnedList[i] != null)
            {
                CurrentEnemySpawnedList[i].SetHealth(0);
            }
        }
    }

    private void UpdateRemoveEnemyList()
    {
        for (int i = GetTotalCurrentEnemySpawnedList() - 1; i >= 0; i--)
        {
            if (CurrentEnemySpawnedList[i] == null)
            {
                CurrentEnemySpawnedList.RemoveAt(i);
            }
            else
            {
                if (CurrentEnemySpawnedList[i].IsDead())
                {
                    CurrentEnemySpawnedList.RemoveAt(i);
                }
            }
        }

        for (int i = BossEnemyList.Count - 1; i >= 0; i--)
        {
            if (BossEnemyList[i] == null)
            {
                BossEnemyList.RemoveAt(i);
            }
            else
            {
                if (BossEnemyList[i].IsDead())
                {
                    OnBossEnemyRemove?.Invoke(BossEnemyList[i]);
                    BossEnemyList.RemoveAt(i);
                }
            }
        }
    }
    public void SetCurrentWave(int value)
    {
        CurrentWave = value;
        ResetCounter();
        OnEnemyWaveChange?.Invoke();
    }

    public int GetCurrentWave()
    {
        return CurrentWave;
    }

    public int GetCurrentEnemyDefeated()
    {
        return CurrentEnemyDefeated;
    }

    public void SetCurrentEnemyDefeated(int value)
    {
        CurrentEnemyDefeated = value;
        OnEnemyDefeatedChange?.Invoke();
    }

    public void ResetTotalEnemiesDefeated()
    {
        TotalEnemiesDefeated = 0;
    }

    public int GetTotalEnemyDefeated()
    {
        return TotalEnemiesDefeated;
    }


    public void ResetCounter()
    {
        SetCurrentEnemyDefeated(0);
    }

    public void ResetEverything()
    {
        ResetTotalEnemiesDefeated();
        SetEnemiesCount(0);
        SetCurrentWave(0);
    }
}
