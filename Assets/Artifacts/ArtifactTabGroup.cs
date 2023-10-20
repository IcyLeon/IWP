using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactTabGroup : MonoBehaviour
{
    List<ArtifactTabButton> tabs = new List<ArtifactTabButton>();
    public event EventHandler onTabChanged;

    [Serializable]
    public struct TabMenu
    {
        public GameObject TabPanel;
        public ArtifactType ArtifactType;
    }
    [SerializeField] TabMenu[] TabMenuList;
    private ArtifactTabButton selectedtab;
    private TabMenu currentPanel;

    [Header("Slider")]
    [SerializeField] Scrollbar slider;
    [SerializeField] HorizontalLayoutGroup TabButtonsLayoutGroup;

    public TabMenu[] GetTabMenuList()
    {
        return TabMenuList;
    }

    public TabMenu GetCurrentTabPanel()
    {
        return currentPanel;
    }

    public int GetTabPanelIdx(ArtifactType ArtifactType)
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            if (TabMenuList[i].ArtifactType == ArtifactType)
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
        currentPanel = TabMenuList[GetTabPanelIdx(selectedtab.artifactType)];
        currentPanel.TabPanel.SetActive(true);
    }

    void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].artifactType == ArtifactType.FLOWER)
                OnTabSelected(tabs[i]);
        }
        InitSliderSize();
    }

    void InitSliderSize()
    {
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(tabs.Count * (70f + TabButtonsLayoutGroup.spacing), slider.GetComponent<RectTransform>().sizeDelta.y);
        slider.size = 1f / tabs.Count;
    }

    public ArtifactTabButton GetTabSelected()
    {
        return selectedtab;
    }

    public void Subscribe(ArtifactTabButton tb)
    {
        tabs.Add(tb);
    }

    public void OnTabSelected(ArtifactTabButton tb)
    {
        OnTabReset();
        selectedtab = tb;
        selectedtab.SelectedTabIcons();
        OpenPanel();
        StartCoroutine(MoveScrollBar());
        onTabChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnTabReset()
    {
        foreach(ArtifactTabButton tb in tabs)
        {
            tb.ResetTabIcons();
        }
    }

    IEnumerator MoveScrollBar()
    {
        float targetValue = ((float)ArrayUtility.IndexOf(tabs.ToArray(), selectedtab) / (tabs.Count - 1));
        float elapsedTime = 0f;
        float animationDuration = 0.15f;

        while (!Mathf.Approximately(slider.value, targetValue))
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
