using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCanvasUI : MonoBehaviour
{
    private AssetManager assetManager;
    [Header("Player Content")]
    [SerializeField] HealthBarScript PlayerHealthBarREF;
    [SerializeField] StaminaScript StaminaBarREF;

    [Header("Bow")]
    [SerializeField] GameObject BowCrossHair;

    [Header("Interaction Content")]
    [SerializeField] InteractionContentUI InteractOptionsUI;

    [Header("Arrow Indication")]
    [SerializeField] Transform ArrowIndicatorPivot;

    [Header("Player Manager")]
    [SerializeField] PlayerManager playerManager;
    private Dictionary<GameObject, ArrowIndicator> ArrowIndicatorList = new();

    [SerializeField] GameObject[] HideUIList;

    // Start is called before the first frame update
    void Awake()
    {
        AssetManager.OnSpawnArrow += SpawnArrowIndicator;
    }
    private void Start()
    {
        assetManager = AssetManager.GetInstance();
        ShowCrossHair(false);
    }

    public PlayerManager GetPlayerManager()
    {
        return playerManager;
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

    public void ShowCrossHair(bool val)
    {
        BowCrossHair.gameObject.SetActive(val);
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
        for (int i = ArrowIndicatorList.Count - 1; i >= 0; i--)
        {
            KeyValuePair<GameObject, ArrowIndicator> ArrowIndicatorKeyValuePair = ArrowIndicatorList.ElementAt(i);
            if (ArrowIndicatorList.TryGetValue(ArrowIndicatorKeyValuePair.Key, out ArrowIndicator a))
            {
                if (a.GetSource() == null)
                    ArrowIndicatorList.Remove(ArrowIndicatorKeyValuePair.Key);
            }
        }
    }

    private void SpawnArrowIndicator(GameObject source, Color32 color)
    {
        if (isExistInList(source) == null)
        {
            ArrowIndicator a = assetManager.SpawnArrowIndicator(source, playerManager.gameObject, ArrowIndicatorPivot);
            a.SetArrowColor(color);
            ArrowIndicatorList.Add(source, a);
        }
    }

    private ArrowIndicator isExistInList(GameObject source)
    {
        if (ArrowIndicatorList.TryGetValue(source, out ArrowIndicator ArrowIndicator)) {
            return ArrowIndicator;
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
