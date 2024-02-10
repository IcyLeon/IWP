using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabContentManager : MonoBehaviour
{
    [Header("Tab Toggle")]
    [SerializeField] ToggleGroup TabToggleGroup;
    [SerializeField] GameObject[] TabContent;
    [SerializeField] Color32 SelectedColor, DefaultColor;
    [SerializeField] bool IncreaseSizeWhenSelected;
    private Toggle[] TabToggleGroupList;

    // Start is called before the first frame update
    void Awake()
    {
        TabToggleGroupList = TabToggleGroup.GetComponentsInChildren<Toggle>();
        foreach (var tabToggle in TabToggleGroupList)
        {
            ResetToggle(tabToggle);

            int index = TabToggleGroupList.ToList().IndexOf(tabToggle);
            tabToggle.onValueChanged.AddListener(value => ToggleDetails(index));
        }

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
            if (IncreaseSizeWhenSelected)
                toggle.transform.localScale = new Vector3(1.3f, 1.3f, 1);
            toggle.transform.GetComponent<TextMeshProUGUI>().color = SelectedColor;
        }
        else
        {
            ResetToggle(toggle);
        }
    }

    void ResetToggle(Toggle toggle)
    {
        toggle.transform.localScale = Vector3.one;
        toggle.transform.GetComponent<TextMeshProUGUI>().color = DefaultColor;
    }
}
