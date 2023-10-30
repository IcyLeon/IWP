using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public interface IToggle
{
    void ToggleSelection(bool toggle);
}

public class AssetManager : MonoBehaviour
{
    [SerializeField] MessagePanel InfomationPanel;
    [SerializeField] GameObject PopupPanelPrefab;
    [SerializeField] ItemsList itemlisttemplate;
    [SerializeField] GameObject WorldText;
    [SerializeField] GameObject SlashPrefab;

    [Header("Normal Attack Bow")]
    [SerializeField] GameObject CrossHair;
    [SerializeField] GameObject HitEffect;
    [SerializeField] GameObject WorldUIContainer;

    [Header("UI")]
    public GameObject EnemyHealthUIPrefab;
    public GameObject StarPrefab;
    private GameObject DraggingItem;
    private PopupPanel PopupPanel;
    public GameObject ItemBorderPrefab;
    public GameObject SlotPrefab;
    public GameObject ElementalContainerPrefab;
    public GameObject ElementalContainerUIPrefab;

    public GameObject ElementalOrbPrefab;

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

    public void OpenPopupPanel(string text)
    {
        if (PopupPanel != null)
            Destroy(PopupPanel.gameObject);

        PopupPanel = Instantiate(PopupPanelPrefab, GetCanvasGO().transform).GetComponent<PopupPanel>();
        PopupPanel.SetMessage(text);
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

    public GameObject SpawnSlashEffect(Vector3 pos, Quaternion angle)
    {
        GameObject slash = Instantiate(SlashPrefab, pos, angle);
        Destroy(slash, 0.5f);
        return slash;
    }
}
