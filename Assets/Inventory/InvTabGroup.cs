using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InvTabGroup : MonoBehaviour
{
    List<InvTabButton> tabs = new List<InvTabButton>();
    public event EventHandler onTabChanged;

    [Serializable]
    public class TabMenu
    {
        public GameObject TabPanel;
        public Category Category;
    }
    [SerializeField] TabMenu[] TabMenuList;
    private InvTabButton selectedtab;
    private TabMenu currentPanel;


    public TabMenu[] GetTabMenuList()
    {
        return TabMenuList;
    }

    public TabMenu GetCurrentTabPanel()
    {
        return currentPanel;
    }

    public int GetTabPanelIdx(Category Category)
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            if (TabMenuList[i].Category == Category)
            {
                return i;
            }
        }
        return -1;
    }

    private void OpenPanel()
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            TabMenuList[i].TabPanel.SetActive(false);
        }
        currentPanel = TabMenuList[GetTabPanelIdx(selectedtab.Category)];
        currentPanel.TabPanel.SetActive(true);
    }

    void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].Category == Category.FOOD)
                OnTabSelected(tabs[i]);
        }
    }

    public InvTabButton GetTabSelected()
    {
        return selectedtab;
    }

    public void Subscribe(InvTabButton tb)
    {
        tabs.Add(tb);
    }

    public void OnTabSelected(InvTabButton tb)
    {
        OnTabReset();
        selectedtab = tb;
        selectedtab.SelectedTabIcons();
        OpenPanel();
        onTabChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnTabReset()
    {
        foreach (InvTabButton tb in tabs)
        {
            tb.ResetTabIcons();
        }
    }

}
