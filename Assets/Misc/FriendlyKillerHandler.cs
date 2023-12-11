using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyKillerHandler : MonoBehaviour
{
    [Serializable]
    public class FriendlyKillerInfo
    {
        public GameObject FriendlyKillerPrefab;
        public FriendlyKillerSO FriendlyKillerSO;
    }
    [SerializeField] FriendlyKillerInfo[] FriendlyKillerInfoList;
    private int MaxKillers = 3;
    private List<FriendlyKillerData> FriendlyKillerDataList;
    public delegate void OnFriendlyKillersDataChanged(FriendlyKillerData FriendlyKillerData);
    public OnFriendlyKillersDataChanged OnFriendlyKillersDataAdd;
    public OnFriendlyKillersDataChanged OnFriendlyKillersDataRemove;
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
        return FriendlyKillerDataList.Count >= MaxKillers;
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
