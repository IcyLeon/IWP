using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasUI : MonoBehaviour
{
    [Header("Player Content")]
    [SerializeField] HealthBarScript PlayerHealthBarREF;
    [SerializeField] StaminaScript StaminaBarREF;

    [Header("Interaction Content")]
    [SerializeField] InteractionContentUI InteractOptionsUI;

    [Header("Arrow Indication")]
    [SerializeField] Transform ArrowIndicatorPivot;

    [Header("Player Manager")]
    [SerializeField] PlayerManager playerManager;
    private List<ArrowIndicator> ArrowIndicatorList = new();

    [SerializeField] GameObject[] HideUIList;

    // Start is called before the first frame update
    void Awake()
    {
        AssetManager.OnSpawnArrow += SpawnArrowIndicator;
    }

    private void OnDestroy()
    {
        AssetManager.OnSpawnArrow -= SpawnArrowIndicator;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateContent();
        RemoveNullReferenceForArrowList();
    }

    public InteractionContentUI GetInteractOptionsUI()
    {
        return InteractOptionsUI;
    }

    private void UpdateContent()
    {
        foreach (var go in HideUIList)
            go.SetActive(Time.timeScale != 0);
    }
    private void RemoveNullReferenceForArrowList()
    {
        for (int i = ArrowIndicatorList.Count - 1; i > 0; i--)
        {
            ArrowIndicator a = ArrowIndicatorList[i];
            if (a.GetSource() == null)
                ArrowIndicatorList.Remove(a);
        }
    }

    private void SpawnArrowIndicator(GameObject source, Color32 color)
    {
        AssetManager assetManager = AssetManager.GetInstance();
        if (isExistInList(source) == null)
        {
            ArrowIndicator a = assetManager.SpawnArrowIndicator(source, playerManager.gameObject, ArrowIndicatorPivot);
            a.SetArrowColor(color);
            ArrowIndicatorList.Add(a);
        }
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

    public HealthBarScript GetPlayerHealthBar()
    {
        return PlayerHealthBarREF;
    }

    public StaminaScript GetStaminaBar()
    {
        return StaminaBarREF;
    }
}
