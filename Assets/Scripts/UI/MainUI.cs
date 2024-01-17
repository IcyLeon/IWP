using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private static MainUI instance;

    [SerializeField] GameObject[] HideUIList;
    [SerializeField] GameObject FallenUI;
    [SerializeField] GameObject[] MainUIList;
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
    }

    private void Update()
    {
        UpdatePause();
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
}
