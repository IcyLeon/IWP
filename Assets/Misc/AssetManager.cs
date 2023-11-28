using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
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

    [Header("UI")]
    public GameObject EnemyHealthUIPrefab;
    public GameObject StarPrefab;
    private GameObject DraggingItem;
    private PopupPanel PopupPanel;
    private MessageNotification MessageNotification;
    public GameObject ItemBorderPrefab;
    public GameObject ShopInfoPrefab;
    public GameObject SlotPrefab;
    public GameObject ElementalContainerPrefab;
    public GameObject ElementalContainerUIPrefab;
    public GameObject ElementalOrbPrefab;

    [Header("Currency")]
    public Sprite Coins;
    public Sprite Cash;

    [Header("UI")]
    public GameObject ElectricEffect;

    private static AssetManager instance;

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

    public GameObject GetCrossHair()
    {
        return CrossHair;
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
        if (MessageNotification != null)
            Destroy(MessageNotification.gameObject);

        MessageNotification = Instantiate(MessageNotificationPrefab, GetCanvasGO().transform).GetComponent<MessageNotification>();
        MessageNotification.SetMessage(text);
    }

    public void SpawnParticlesEffect(Vector3 position, GameObject prefab)
    {
        ParticleSystem ps = Instantiate(prefab, position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
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
        DamageText dt = Instantiate(WorldText, GetCanvasGO().transform).GetComponent<DamageText>();
        dt.SpawnText(position, element, text);
    }

    public void SpawnWorldText_ElementalReaction(Vector3 position, ElementalReactionState element, string text)
    {
        DamageText dt = Instantiate(WorldText, GetCanvasGO().transform).GetComponent<DamageText>();
        dt.SpawnText(position, element, text);
    }

    public GameObject SpawnSlashEffect(Transform source)
    {
        GameObject slash = Instantiate(SlashPrefab, source);
        slash.transform.SetParent(null);
        Destroy(slash, 0.5f);
        return slash;
    }
}
