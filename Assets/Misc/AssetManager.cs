using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToggle
{
    void ToggleSelection(bool toggle);
}

public class AssetManager : MonoBehaviour
{
    [SerializeField] MessagePanel InfomationPanel;
    [SerializeField] PopupPanel PopupPanel;
    [SerializeField] ItemsList itemlisttemplate;

    [Header("Normal Attack Bow")]
    [SerializeField] GameObject CrossHair;
    [SerializeField] GameObject HitEffect;

    public GameObject StarPrefab;
    private GameObject DraggingItem;
    public GameObject ItemBorderPrefab;
    public GameObject SlotPrefab;

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

}
