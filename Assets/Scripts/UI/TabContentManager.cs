using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TabContentManager : MonoBehaviour
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
            int index = ArrayUtility.IndexOf(TabToggleGroupList, tabToggle);
            tabToggle.onValueChanged.AddListener(value => ToggleDetails(index));
        }
    }
    private void Start()
    {
        Toggle(TabToggleGroup.GetFirstActiveToggle());
    }
    void ToggleDetails(int idx)
    {
        TabContent[idx].gameObject.SetActive(TabToggleGroupList[idx].isOn);
        Toggle(TabToggleGroupList[idx]);
    }

    void Toggle(Toggle toggle)
    {
        if (toggle.isOn)
        {
            toggle.transform.localScale = new Vector3(1.3f, 1.3f, 1);
            toggle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = SelectedColor;
        }
        else
        {
            toggle.transform.localScale = Vector3.one;
            toggle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = DefaultColor;
        }
    }
}
