using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactTabGroup : MonoBehaviour
{
    public event Action onTabChanged;
    private Coroutine MovingScrollbar;

    [Serializable]
    public struct TabMenu
    {
        public GameObject TabPanel;
        public ArtifactType ArtifactType;
        public ArtifactTabButton TabButton;
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

    public int GetTabPanelIdx(ArtifactTabButton atb)
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            if (TabMenuList[i].TabButton == atb)
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
        currentPanel = TabMenuList[GetTabPanelIdx(selectedtab)];
        currentPanel.TabPanel.SetActive(true);
    }

    void Start()
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            if (TabMenuList[i].ArtifactType == ArtifactType.FLOWER)
                OnTabSelected(TabMenuList[i].TabButton);
        }
        InitSliderSize();
    }

    void InitSliderSize()
    {
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(TabMenuList.Length * (70f + TabButtonsLayoutGroup.spacing), slider.GetComponent<RectTransform>().sizeDelta.y);
        slider.size = 1f / TabMenuList.Length;
    }

    public ArtifactTabButton GetTabSelected()
    {
        return selectedtab;
    }

    public int GetTabMenuByArtifactType(ArtifactType artifactType)
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            if (TabMenuList[i].ArtifactType == artifactType)
                return i;
        }
        return -1;
    }

    public void OnTabSelected(ArtifactTabButton tb)
    {
        OnTabReset();
        selectedtab = tb;
        selectedtab.SelectedTabIcons();
        OpenPanel();

        float targetValue = ((float)(GetTabPanelIdx(selectedtab)) / (TabMenuList.Length - 1));

        if (MovingScrollbar != null)
        {
            StopCoroutine(MovingScrollbar);
        }

        if (gameObject.activeInHierarchy)
            MovingScrollbar = StartCoroutine(MoveScrollBar(targetValue));
        else
            slider.value = targetValue;

        onTabChanged?.Invoke();
    }

    public void OnTabReset()
    {
        foreach(TabMenu tb in TabMenuList)
        {
            tb.TabButton.ResetTabIcons();
        }
    }

    IEnumerator MoveScrollBar(float target)
    {
        //float targetValue = ((tabs.Count - (float)tabs.IndexOf(selectedtab) - 1) / (tabs.Count - 1));
        float elapsedTime = 0f;
        float animationDuration = 0.15f;

        while (!Mathf.Approximately(slider.value, target))
        {
            slider.value = Mathf.Lerp(slider.value, target, elapsedTime / animationDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
