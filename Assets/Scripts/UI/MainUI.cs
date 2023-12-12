using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    private static MainUI instance;

    [SerializeField] HealthBarScript PlayerHealthBarREF;
    [SerializeField] StaminaScript StaminaBarREF;
    [SerializeField] Transform ElementalDisplayUITransform;
    [SerializeField] InteractionContentUI InteractOptionsUI;
    [SerializeField] GameObject CombatUI;
    [SerializeField] GameObject FallenUI;
    [SerializeField] GameObject[] MainUIList;
    [SerializeField] UpgradeCanvas upgradeCanvas;
    [SerializeField] GameObject ItemBag;
    private List<ArrowIndicator> ArrowIndicatorList;
    [SerializeField] GameObject BlueEffectObject;
    private bool Paused;

    public static MainUI GetInstance()
    {
        return instance;
    }

    public UpgradeCanvas GetUpgradeCanvas()
    {
        return upgradeCanvas;
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
        Paused = isMainUIList();
        CombatUI.SetActive(!isPaused());

        if (isPaused() || FallenPanelIsOpen())
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        if (BlueEffectObject)
            BlueEffectObject.SetActive(isPaused());
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
