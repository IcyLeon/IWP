using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class MainUI : MonoBehaviour
{
    private static MainUI instance;

    [SerializeField] GameObject PlayerHealthBarREF;
    [SerializeField] Transform ElementalDisplayUITransform;
    [SerializeField] InteractionContentUI InteractOptionsUI;
    [SerializeField] GameObject CombatUI;
    [SerializeField] GameObject[] MainUIList;
    private List<ArrowIndicator> ArrowIndicatorList; 

    public static MainUI GetInstance()
    {
        return instance;
    }

    public InteractionContentUI GetInteractOptionsUI()
    {
        return InteractOptionsUI;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        ArrowIndicatorList = new();
    }

    private void Update()
    {
        CombatUI.SetActive(!isMainUIList());
        RemoveNullReferenceForArrowList();
    }

    public bool isMainUIList()
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
        return PlayerHealthBarREF.GetComponent<HealthBarScript>();
    }
}
