using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IToggle
{
    void ToggleSelection(bool toggle);
}

public class AssetManager : MonoBehaviour
{
    [SerializeField] GameObject ArrowIndicatorPrefab;

    [Header("Panels")]
    [SerializeField] GameObject FoodPanelPrefab;
    [SerializeField] GameObject PopupPanelPrefab;
    [SerializeField] GameObject MessageNotificationPrefab;

    [Header("Others")]
    [SerializeField] ItemsList itemlisttemplate;
    [SerializeField] GameObject DamageText;
    [SerializeField] GameObject WorldText;
    [SerializeField] GameObject SlashPrefab;
    [SerializeField] GameObject ObtainUIPrefab;
    public GameObject SwitchCharacterParticlesEffect;
    public GameObject PlungeParticlesEffect;

    [Header("Bow")]
    public GameObject CrossHair;
    public GameObject HitEffect;
    public GameObject HitExplosion;
    public ParticleSystem ChargeUpEmitterPrefab;
    [Header("Normal Attack Sword")]
    public GameObject BasicAttackHitEffect;

    [Header("Amber")]
    public GameObject ESkillArrowsPrefab;
    public GameObject CoordinateAttackPrefab;
    [Header("Kaqing")]
    public GameObject ElectroSlashPrefab;

    [Header("UI")]
    public GameObject EnemyHealthUIPrefab;
    public GameObject StarPrefab;
    private GameObject DraggingItem;
    private PopupPanel PopupPanel;
    public GameObject ItemBorderPrefab;
    public GameObject ShopInfoPrefab;
    public GameObject SlotPrefab;
    public GameObject ElementalContainerPrefab;
    public GameObject ElementalContainerUIPrefab;
    public GameObject ElementalOrbPrefab;

    public GameObject IconPrefab;

    [Header("Currency")]
    public Sprite Coins;
    public Sprite Cash;

    [Header("Effects")]
    public GameObject ElectricEffect;
    public GameObject HealEffect;

    [Header("FriendlyKillers")]
    public GameObject FirePrefab;

    [Header("Dash")]
    [SerializeField] GameObject DashPrefab;

    [Header("InteractableItemObject")]
    [SerializeField] GameObject InteractableItemObjectPrefab;
    [SerializeField] GameObject ItemCollectedUIPrefab;

    private static AssetManager instance;

    public static Action<GameObject, Color32> OnSpawnArrow = delegate { };

    public static void CallSpawnArrow(GameObject GameObject, Color32 color)
    {
        OnSpawnArrow?.Invoke(GameObject, color);
    }

    public static bool isInProbabilityRange(float a)
    {
        float randomValue = Random.value;
        float probability = 1.0f - a;
        return randomValue > probability;
    }

    public static IEnumerator DisableDissolved(GameObject go, Material materialInstance, float timer = 1f)
    {
        float startTime = Time.time;
        float timetaken = timer;
        float value = 0f;

        while (Time.time - startTime < timetaken)
        {
            float t = (Time.time - startTime) / timetaken;
            value = Mathf.Lerp(value, 1f, t);

            materialInstance.SetFloat("_Scale", value);
            yield return null;
        }

        materialInstance.SetFloat("_Scale", 1f); // Ensure the final value is set precisely.
    }

    public static IEnumerator Dissolved(GameObject go, Material materialInstance, float timer = 1f)
    {
        float startTime = Time.time;
        float timetaken = timer;
        float value = 1f;

        while (Time.time - startTime < timetaken)
        {
            float t = (Time.time - startTime) / timetaken;
            value = Mathf.Lerp(value, 0f, t);

            materialInstance.SetFloat("_Scale", value);
            yield return null;
        }

        materialInstance.SetFloat("_Scale", 0f);
        Destroy(go.gameObject);
    }

    public static string CurrencyText(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.COINS:
                return "Coins";
            case CurrencyType.CASH:
                return "Cash";
        }
        return "???";
    }

    public void SpawnFoodPanel(Food FoodREF)
    {
        FoodPanel FoodPanel = Instantiate(FoodPanelPrefab, GetCanvasGO().transform).GetComponent<FoodPanel>();
        FoodPanel.SetFoodREF(FoodREF);
        FoodPanel.transform.SetAsLastSibling();
    }

    // Start is called before the first frame update
    void Awake()
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
    }

    public void SpawnItemDropListGO(List<Item> ItemList, Vector3 pos)
    {
        if (ItemList == null)
            return;

        for (int i = 0; i < ItemList.Count; i++)
        {
            Item item = ItemList[i];
            SpawnItemDrop(item, pos);
        }
    }


    public void SpawnItemDrop(Item item, Vector3 spawnPos, ItemTemplate itemTemplate = null) // if item/existing item is null, use itemtemplate to spawn a new item
    {
        InteractableItemObjects itemGO = Instantiate(InteractableItemObjectPrefab, spawnPos, Quaternion.identity).GetComponent<InteractableItemObjects>();
        itemGO.GetComponent<Rigidbody>().AddForce(Vector3.up * 20f, ForceMode.Impulse);
        Item ItemREF = null;
        if (item == null)
        {
            Item newItem = InventoryManager.CreateItem(itemTemplate);
            ItemREF = newItem;
        }
        else
        {
            ItemREF = item;
        }
        itemGO.SetItemsREF(ItemREF);
    }

    public GameObject SpawnCrossHair()
    {
        GameObject go = Instantiate(CrossHair, GetCanvasGO().transform);
        return go;
    }

    public ItemCollectedUI SpawnItemCollectedUI(ItemTemplate itemTemplate)
    {
        ItemCollectedUI ItemCollectedUI = Instantiate(ItemCollectedUIPrefab, GetCanvasGO().transform).GetComponent<ItemCollectedUI>();
        ItemCollectedUI.Init(itemTemplate);
        ItemCollectedUI.transform.SetAsFirstSibling();
        return ItemCollectedUI;
    }

    public GameObject GetDragItem()
    {
        return DraggingItem;
    }
    public void SetDragItem(GameObject go)
    {
        DraggingItem = go;
    }
    public static AssetManager GetInstance()
    {
        return instance;
    }

    public void SpawnObtainedUI()
    {
        GameObject go = Instantiate(ObtainUIPrefab, GetCanvasGO().transform);
    }

    public void SpawnDash(Vector3 pos, Quaternion rot)
    {
        ParticleSystem go = Instantiate(DashPrefab, pos, rot).GetComponent<ParticleSystem>();
        Destroy(go, go.main.duration);
    }
    public Sprite GetCurrencySprite(CurrencyType type)
    {
        switch(type)
        {
            case CurrencyType.COINS:
                return Coins;
            case CurrencyType.CASH:
                return Cash;
        }
        return null;
    }

    public void OpenPopupPanel(string text)
    {
        if (PopupPanel != null)
            Destroy(PopupPanel.gameObject);

        PopupPanel = Instantiate(PopupPanelPrefab, GetCanvasGO().transform).GetComponent<PopupPanel>();
        PopupPanel.transform.SetAsFirstSibling();
        PopupPanel.SetMessage(text);
    }

    public void OpenMessageNotification(string text)
    {
        MessageNotification m = Instantiate(MessageNotificationPrefab, GetCanvasGO().transform).GetComponent<MessageNotification>();
        m.transform.SetAsFirstSibling();
        m.SetMessage(text);
    }

    public void SpawnParticlesEffect(Vector3 position, GameObject prefab)
    {
        ParticleSystem ps = Instantiate(prefab, position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
    }

    public WorldText SpawnWorldText(Sprite sprite, string val, Transform Parent)
    {
        WorldText wt = Instantiate(WorldText).GetComponent<WorldText>();
        wt.transform.SetParent(Parent, true);
        wt.transform.localPosition = Vector3.zero;
        wt.UpdateContent(sprite, val);
        return wt;
    }
    public ArrowIndicator SpawnArrowIndicator(GameObject target, GameObject source, Transform t)
    {
        ArrowIndicator go = Instantiate(ArrowIndicatorPrefab, t).GetComponent<ArrowIndicator>();
        go.SetTarget(target);
        go.SetSource(source);
        go.transform.SetAsFirstSibling();
        return go;
    }

    public Canvas GetCanvasGO()
    {
        GameObject go = GameObject.Find("Canvas");
        if (go == null)
            return null;

        return go.GetComponent<Canvas>();
    }

    public void OpenMessagePanel(string headtext, string bodytext)
    {
        //PopupPanel.SetMessage(text);
    }
    public ItemsList GetItemListTemplate()
    {
        return itemlisttemplate;
    }
    public void UpdateCurrentSelectionOutline(IToggle prev, IToggle current)
    {
        if (prev != null)
        {
            prev.ToggleSelection(false);
        }
        if (current != null)
        {
            current.ToggleSelection(true);
        }
    }

    public static Vector3 RandomVectorInCone(Vector3 front, float Angle)
    {
        float randomPitch = Random.Range(-Angle / 2, Angle / 2);
        float randomYaw = Random.Range(-Angle / 2, Angle / 2);
        Quaternion Adjustment = Quaternion.Euler(randomPitch, randomYaw, 0);
        Vector3 randomDirection = Adjustment * front;
        return randomDirection.normalized;
    }

    public void SpawnWorldText_Elemental(Vector3 position, Elemental element, string text)
    {
        DamageText dt = Instantiate(DamageText, GetCanvasGO().transform).GetComponent<DamageText>();
        dt.transform.SetAsFirstSibling();
        dt.SpawnText(position, element, text);
    }

    public void SpawnWorldText_Other(Vector3 position, OthersState OthersState, string text)
    {
        DamageText dt = Instantiate(DamageText, GetCanvasGO().transform).GetComponent<DamageText>();
        dt.transform.SetAsFirstSibling();
        dt.SpawnText(position, OthersState, text);
    }

    public void SpawnWorldText_ElementalReaction(Vector3 position, ElementalReactionState element, string text)
    {
        DamageText dt = Instantiate(DamageText, GetCanvasGO().transform).GetComponent<DamageText>();
        dt.transform.SetAsFirstSibling();
        dt.SpawnText(position, element, text);
    }

    public void SpawnSlashEffect(Transform source)
    {
        GameObject slash = Instantiate(SlashPrefab, source);
        slash.transform.SetParent(null);
        Destroy(slash, 0.5f);
    }

    public void SpawnSlashEffect(GameObject prefab, Transform source)
    {
        GameObject slash = Instantiate(prefab, source);
        slash.transform.SetParent(null);
        Destroy(slash, 0.5f);
    }
}
