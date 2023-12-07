using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyKillerHandler : MonoBehaviour
{
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

    public List<FriendlyKillerData> GetFriendlyKillerDataList()
    {
        return FriendlyKillerDataList;
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
