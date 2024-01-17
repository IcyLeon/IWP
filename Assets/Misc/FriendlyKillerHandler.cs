using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FriendlyKillerHandler : MonoBehaviour
{
    [Serializable]
    public class FriendlyKillerInfo
    {
        public GameObject FriendlyKillerPrefab;
        public FriendlyKillerSO FriendlyKillerSO;
    }
    [SerializeField] FriendlyKillerInfo[] FriendlyKillerInfoList;
    private int MaxKillersLimit = 3;
    private int PossibleMaxTurretPerWave = 10;
    private List<FriendlyKillerData> FriendlyKillerDataList;
    public delegate void OnFriendlyKillersDataChanged(FriendlyKillerData FriendlyKillerData);
    public static OnFriendlyKillersDataChanged OnFriendlyKillersDataAdd;
    public static OnFriendlyKillersDataChanged OnFriendlyKillersDataRemove;
    private static FriendlyKillerHandler instance;

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

        FriendlyKillerDataList = new();
        FallenUI.OnReviveChange += RemoveAllTurrets;
        SceneManager.OnSceneChanged += OnSceneChanged;
    }

    void RemoveAllTurrets()
    {
        for (int i = 0; i < GetFriendlyKillerDataList().Count; i++)
        {
            RemoveKillerToList(GetFriendlyKillerDataList()[i]);
        }
    }

    private void OnDestroy()
    {
        FallenUI.OnReviveChange -= RemoveAllTurrets;
        SceneManager.OnSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(SceneEnum sceneEnum)
    {
        switch (sceneEnum)
        {
            case SceneEnum.GAME:
                SpawnTurretsOnTerrains();
                break;
        }
    }

    private void SpawnTurretsOnTerrains()
    {
        GameObject terrainGO = GameObject.FindGameObjectWithTag("Terrain");
        if (terrainGO == null)
            return;

        Terrain terrain = terrainGO.GetComponent<Terrain>();
        if (terrain == null)
            return;


        int randomAmt = Random.Range(0, PossibleMaxTurretPerWave + 1);
        for (int i = 0; i < randomAmt; i++)
        {
            int RandomKillers = Random.Range(0, FriendlyKillerInfoList.Length);
            Vector3 pos = EnemyManager.GetRandomPointWithinTerrain(terrain);
            FriendlyKillers friendlyKillers = Instantiate(FriendlyKillerInfoList[RandomKillers].FriendlyKillerPrefab, pos, Quaternion.identity).GetComponent<FriendlyKillers>();
        }
    }

    public void LoadKillersAroundTerrain(Terrain terrain)
    {
        foreach (FriendlyKillerData f in GetFriendlyKillerDataList())
        {
            Vector3 pos = EnemyManager.GetRandomPointWithinTerrain(terrain);
            FriendlyKillers friendlyKillers = Instantiate(GetFriendlyKillerInfo(f.GetFriendlyKillerSO()).FriendlyKillerPrefab, pos, Quaternion.identity).GetComponent<FriendlyKillers>();
            if (friendlyKillers != null)
                friendlyKillers.SetKillerData(f);
        }
    }

    public FriendlyKillerInfo GetFriendlyKillerInfo(FriendlyKillerSO fSO)
    {
        for (int i = 0; i < FriendlyKillerInfoList.Length; i++)
        {
            if (FriendlyKillerInfoList[i].FriendlyKillerSO == fSO)
                return FriendlyKillerInfoList[i];
        }
        return null;
    }

    public List<FriendlyKillerData> GetFriendlyKillerDataList()
    {
        return FriendlyKillerDataList;
    }

    public bool HasReachedLimitKillers()
    {
        return FriendlyKillerDataList.Count >= MaxKillersLimit;
    }

    public static FriendlyKillerHandler GetInstance()
    {
        return instance;
    }
    public void AddKillerToList(FriendlyKillerData f)
    {
        if (f == null)
            return;

        FriendlyKillerDataList.Add(f);
        OnFriendlyKillersDataAdd?.Invoke(f);
    }

    public void RemoveKillerToList(FriendlyKillerData f)
    {
        if (FriendlyKillerDataList.Remove(f))
        {
            OnFriendlyKillersDataRemove?.Invoke(f);
        }
    }
}
