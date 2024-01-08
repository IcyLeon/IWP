using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private static MainUI instance;

    [SerializeField] HealthBarScript PlayerHealthBarREF;
    [SerializeField] StaminaScript StaminaBarREF;
    [SerializeField] Transform ElementalDisplayUITransform;
    [SerializeField] InteractionContentUI InteractOptionsUI;
    [SerializeField] GameObject[] HideUIList;
    [SerializeField] GameObject FallenUI;
    [SerializeField] GameObject[] MainUIList;
    [SerializeField] UpgradeCanvas upgradeCanvas;
    [SerializeField] DisplayItemsStatsManager displayItemsStatsManager;
    [SerializeField] GameObject ItemBag;
    private List<ArrowIndicator> ArrowIndicatorList;
    [SerializeField] GameObject BlueEffectObject;

    private bool Paused;

    public void SetPaused(bool val)
    {
        Paused = val;
    }
    public static void SetProgressUpgrades(Slider slider, float min, float max)
    {
        slider.minValue = min;
        slider.maxValue = max;
    }
    public static MainUI GetInstance()
    {
        return instance;
    }

    public UpgradeCanvas GetUpgradeCanvas()
    {
        return upgradeCanvas;
    }

    public DisplayItemsStatsManager GetDisplayItemsStatsManager()
    {
        return displayItemsStatsManager;
    }
    public InteractionContentUI GetInteractOptionsUI()
    {
        return InteractOptionsUI;
    }

    public void OpenItemBag()
    {
        ItemBag.SetActive(true);
    }


    public void OpenFallenPanel()
    {
        if (!FallenPanelIsOpen())
        {
            FallenUI.gameObject.SetActive(true);
        }
    }

    public bool FallenPanelIsOpen()
    {
        return FallenUI.gameObject.activeSelf;
    }

    private void Awake()
    {
        instance = this;
        ArrowIndicatorList = new();
    }

    private void Update()
    {
        UpdatePause();
        RemoveNullReferenceForArrowList();
    }

    public bool isCursorVisible()
    {
        return Cursor.visible;
    }

    private void UpdatePause()
    {
        foreach(var go in HideUIList)
            go.SetActive(Time.timeScale != 0);

        if (FallenPanelIsOpen() || isMainUIList() || isPaused())
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        if (BlueEffectObject)
            BlueEffectObject.SetActive(isPaused() || Time.timeScale == 0);
    }

    public bool isPaused()
    {
        return Paused;
    }

    private bool isMainUIList()
    {
        for(int i = 0; i < MainUIList.Length; i++)
        {
            if (MainUIList[i].activeSelf)
                return true;
        }
        return false;
    }

    private ArrowIndicator isExistInList(GameObject source)
    {
        for (int i = 0; i < ArrowIndicatorList.Count; i++)
        {
            ArrowIndicator a = ArrowIndicatorList[i];
            if (a.GetSource() == source)
                return a;
        }
        return null;
    }

    public void RemoveNullReferenceForArrowList()
    {
        for (int i = ArrowIndicatorList.Count - 1; i > 0; i--)
        {
            ArrowIndicator a = ArrowIndicatorList[i];
            if (a.GetSource() == null)
                ArrowIndicatorList.Remove(a);
        }
    }

    public void SpawnArrowIndicator(GameObject source)
    {
        AssetManager assetManager = AssetManager.GetInstance();
        if (isExistInList(source) == null)
        {
            ArrowIndicator a = assetManager.SpawnArrowIndicator(source);
            ArrowIndicatorList.Add(a);
        }
    }

    public Transform GetElementalDisplayUITransform()
    {
        return ElementalDisplayUITransform;
    }

    public HealthBarScript GetPlayerHealthBar()
    {
        return PlayerHealthBarREF;
    }

    public StaminaScript GetStaminaBar()
    {
        return StaminaBarREF;
    }
}
