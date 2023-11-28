using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopTabContentManager : MonoBehaviour
{
    [Header("Tab Toggle")]
    [SerializeField] ToggleGroup TabToggleGroup;
    [SerializeField] GameObject[] TabContent;
    [SerializeField] Color32 SelectedColor, DefaultColor;
    private Toggle[] TabToggleGroupList;

    // Start is called before the first frame update
    void Awake()
    {
        TabToggleGroupList = TabToggleGroup.GetComponentsInChildren<Toggle>();
        foreach (var tabToggle in TabToggleGroupList)
        {
            ResetToggle(tabToggle);

            int index = ArrayUtility.IndexOf(TabToggleGroupList, tabToggle);
            tabToggle.onValueChanged.AddListener(value => ToggleDetails(index));
        }

        if (TabToggleGroupList.Length > 0)
        {
            TabToggleGroupList[0].isOn = true;
        }
    }

    public GameObject[] GetTabContent()
    {
        return TabContent;
    }
    private void Start()
    {
        Toggle(TabToggleGroup.GetFirstActiveToggle());
    }
    void ToggleDetails(int idx)
    {
        if (idx >= TabContent.Length)
            return;

        TabContent[idx].gameObject.SetActive(TabToggleGroupList[idx].isOn);
        Toggle(TabToggleGroupList[idx]);
    }

    void Toggle(Toggle toggle)
    {
        if (toggle == null)
            return;

        if (toggle.isOn)
        {
            toggle.transform.GetChild(0).GetComponent<Image>().color = SelectedColor;
            toggle.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = DefaultColor;
        }
        else
        {
            ResetToggle(toggle);
        }
    }

    void ResetToggle(Toggle toggle)
    {
        toggle.transform.GetChild(0).GetComponent<Image>().color = DefaultColor;
        toggle.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = SelectedColor;
    }
}
