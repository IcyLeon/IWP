using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyKillerUI : MonoBehaviour
{
    [SerializeField] GameObject FriendlyKillerPlacementUI;
    [SerializeField] Transform FriendlyKillerInfoPivot;
    private Dictionary<FriendlyKillerData, FriendlyKillerPlacementUI> FriendlyKillerPlacementUI_Dictionary;
    private FriendlyKillerHandler FriendlyKillerHandler;

    private void Awake()
    {
        FriendlyKillerPlacementUI_Dictionary = new();
    }
    // Start is called before the first frame update
    void Start()
    {
        FriendlyKillerHandler = FriendlyKillerHandler.GetInstance();
        FriendlyKillerHandler.OnFriendlyKillersDataAdd += OnFriendlyKillersDataAdd;
        FriendlyKillerHandler.OnFriendlyKillersDataRemove += OnFriendlyKillersDataRemove;
        Init();
    }

    void Init()
    {
        foreach(var go in FriendlyKillerInfoPivot.GetComponentsInChildren<FriendlyKillerPlacementUI>())
            Destroy(go.gameObject);

        foreach (var f in FriendlyKillerHandler.GetFriendlyKillerDataList())
        {
            FriendlyKillerPlacementUI friendlyKillerPlacementUI = Instantiate(FriendlyKillerPlacementUI, FriendlyKillerInfoPivot).GetComponent<FriendlyKillerPlacementUI>();
            friendlyKillerPlacementUI.SetFriendlyKillerREF(f);
            FriendlyKillerPlacementUI_Dictionary.Add(f, friendlyKillerPlacementUI);
        }
    }
    void OnFriendlyKillersDataAdd(FriendlyKillerData f)
    {
        FriendlyKillerPlacementUI friendlyKillerPlacementUI = Instantiate(FriendlyKillerPlacementUI, FriendlyKillerInfoPivot).GetComponent<FriendlyKillerPlacementUI>();
        friendlyKillerPlacementUI.SetFriendlyKillerREF(f);
        FriendlyKillerPlacementUI_Dictionary.Add(f, friendlyKillerPlacementUI);
    }

    void OnFriendlyKillersDataRemove(FriendlyKillerData f)
    {
        Destroy(FriendlyKillerPlacementUI_Dictionary[f].gameObject);
        FriendlyKillerPlacementUI_Dictionary.Remove(f);
    }

    private void OnDestroy()
    {
        FriendlyKillerHandler.OnFriendlyKillersDataAdd -= OnFriendlyKillersDataAdd;
        FriendlyKillerHandler.OnFriendlyKillersDataRemove -= OnFriendlyKillersDataRemove;
    }
}
