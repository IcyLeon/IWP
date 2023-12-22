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

    [SerializeField] MessagePanel InfomationPanel;
    [SerializeField] GameObject PopupPanelPrefab;
    [SerializeField] GameObject MessageNotificationPrefab;
    [SerializeField] ItemsList itemlisttemplate;
    [SerializeField] GameObject DamageText;
    [SerializeField] GameObject WorldText;
    [SerializeField] GameObject SlashPrefab;

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


    private static AssetManager instance;

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

    public GameObject SpawnCrossHair()
    {
        GameObject go = Instantiate(CrossHair, GetCanvasGO().transform);
        return go;
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
        PopupPanel.SetMessage(text);
    }

    public void OpenMessageNotification(string text)
    {
        MessageNotification m = Instantiate(MessageNotificationPrefab, GetCanvasGO().transform).GetComponent<MessageNotification>();
        m.SetMessage(text);
    }

    public void SpawnParticlesEffect(Vector3 position, GameObject prefab)
    {
        ParticleSystem ps = Instantiate(prefab, position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
    }

    public WorldText SpawnWorldText(Sprite sprite, string val, Transform Parent)
    {
        WorldText wt = Instantiate(WorldText, Parent).GetComponent<WorldText>();
        wt.UpdateContent(sprite, val);
        return wt;
    }
    public ArrowIndicator SpawnArrowIndicator(GameObject source)
    {
        ArrowIndicator go = Instantiate(ArrowIndicatorPrefab, GetCanvasGO().transform).GetComponent<ArrowIndicator>();
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
        dt.SpawnText(position, element, text);
    }

    public void SpawnWorldText_Other(Vector3 position, OthersState OthersState, string text)
    {
        DamageText dt = Instantiate(DamageText, GetCanvasGO().transform).GetComponent<DamageText>();
        dt.SpawnText(position, OthersState, text);
    }

    public void SpawnWorldText_ElementalReaction(Vector3 position, ElementalReactionState element, string text)
    {
        DamageText dt = Instantiate(DamageText, GetCanvasGO().transform).GetComponent<DamageText>();
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
