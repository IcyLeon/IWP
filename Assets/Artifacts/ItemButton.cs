using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IToggle
{
    private Color ImageColor;
    [Header("Get Item Reference")]
    private Item itemREF;
    private Sprite itemSprite;

    [Header("Only for item display")]
    // leave itemTemplate empty if not using it for displaying, like showcase; apple information, artifacts in the game
    [SerializeField] ItemTemplate DisplayitemTemplate;
    private ItemTemplate itemTemplate;

    public delegate void SendItemButtonInfo(ItemButton itemButton);
    public event SendItemButtonInfo onButtonClick;
    public event SendItemButtonInfo onButtonRemoveClick;
    public event SendItemButtonInfo onButtonSpawn;
    public event SendItemButtonInfo onButtonUpdate;

    [Header("Icons")]
    [SerializeField] EquippedByCharacter EquipByIconContent;

    [Header("Item")]
    [SerializeField] TextMeshProUGUI DisplayAmountTxt;

    [Header("Button Components")]
    [SerializeField] Image Background;
    [SerializeField] Image OutlineAnimation;
    [SerializeField] Image Outline;
    [SerializeField] Button RemoveImage;
    [SerializeField] Transform StarsTransformParent;
    [SerializeField] GameObject RemoveGO;
    [SerializeField] Image ItemImage;
    [SerializeField] Image NewImage;
    [SerializeField] LockItem lockItem;
    [SerializeField] CanvasGroup canvasGroup;
    //[SerializeField] ParticleSystem Burst;


    private RectTransform ItemButton_Rect;
    private ItemButton ItemButton_Drag;

    public EquippedByCharacter GetEquipByIconContent()
    {
        return EquipByIconContent;
    }
    public RectTransform GetItemButtonRect()
    {
        return ItemButton_Rect;
    }

    public CanvasGroup CanvasGroup
    {
        get { return canvasGroup; }
    }

    public Sprite GetItemImage
    {
        get { return itemSprite; }
    }

    private void Awake()
    {
        ItemButton_Rect = GetComponent<RectTransform>();
        RemoveImage.onClick.AddListener(RemoveItemEvent);
    }

    private void RemoveItemEvent()
    {
        onButtonRemoveClick.Invoke(this);
    }

    public void SetDisplayStars(Rarity rarity)
    {
        for (int i = 0; i <= (int)rarity; i++)
        {
            GameObject go = Instantiate(AssetManager.GetInstance().StarPrefab, StarsTransformParent);
        }
        Background.sprite = AssetManager.GetInstance().GetItemListTemplate().raritylist[(int)rarity].rarityborderimage;
    }

    private void CopyItemButton(Transform parentTransform)
    {
        if (parentTransform == null)
            parentTransform = AssetManager.GetInstance().GetCanvasGO().transform;

        GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, parentTransform, false);
        go.transform.SetAsLastSibling();
        ItemButton_Drag = go.GetComponent<ItemButton>();
        ItemButton_Drag.CanvasGroup.blocksRaycasts = false;
        ItemButton_Drag.SetItemREF(ItemButton_Rect.GetComponent<ItemButton>().GetItemREF());
        ItemButton_Drag.GetItemREF().isNew = false;
        AssetManager.GetInstance().SetDragItem(ItemButton_Drag.transform.gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData, Transform parentTransform)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;

        CopyItemButton(parentTransform);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (ItemButton_Drag == null)
            return;

        ItemButton_Drag.GetItemButtonRect().anchoredPosition = eventData.position - (AssetManager.GetInstance().GetCanvasGO().renderingDisplaySize / 2f);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(ItemButton_Drag.gameObject);
        ItemButton_Drag = null;
    }


    // Update is called once per frame
    void Start()
    {
        SetButtonSprites();
        onButtonSpawn?.Invoke(this);

        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y , 0);
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        UpdateHideLock();

        if (OutlineAnimation.gameObject.activeSelf)
        {
            transform.localScale = new Vector3(1.02f, 1.02f, 1f);
            Outline.gameObject.SetActive(true);
        }

        if (GetItemREF() != null)
        {
            switch (GetItemREF())
            {
                case UpgradableItems upgradableItems:
                    SetDisplayText("+" + upgradableItems.GetLevel());
                    break;
                case ConsumableItem consumableItem:
                    SetDisplayText(consumableItem.GetAmount().ToString());
                    break;
            }
        }
        onButtonUpdate?.Invoke(this);
    }

    public void ToggleRemoveItemImage(bool input)
    {
        RemoveGO.SetActive(input);
    }

    public void SetItemsSO(ItemTemplate ItemsSO)
    {
        itemTemplate = ItemsSO;
    }

    private void UpdateHideLock()
    {
        if (itemREF is UpgradableItems)
        {
            UpgradableItems upgradableItems = (UpgradableItems)itemREF;
            if (!upgradableItems.GetLockStatus())
                lockItem.gameObject.SetActive(false);
            else
            {
                lockItem.gameObject.SetActive(true);
                lockItem.SetItemREF(itemREF);
            }

            return;
        }

        lockItem.gameObject.SetActive(false);
    }
    public void SetButtonSprites()
    {
        if (itemREF != null)
        {
            itemSprite = itemREF.GetItemSO().ItemSprite;
            SetDisplayStars(GetItemREF().GetRarity());

            if (itemREF.isNew)
            {
                NewImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (DisplayitemTemplate != null)
            {
                itemTemplate = DisplayitemTemplate;
            }

            if (itemTemplate)
            {
                itemSprite = itemTemplate.ItemSprite;
                SetDisplayStars(itemTemplate.Rarity);
            }
        }

        ItemImage.sprite = itemSprite;
    }

    public ItemTemplate GetItemsSO()
    {
        return itemTemplate;
    }
    public void DisableNewImage()
    {
        NewImage.gameObject.SetActive(false);
    }

    void IToggle.ToggleSelection(bool toggle)
    {
        OutlineAnimation.gameObject.SetActive(toggle);
        Outline.gameObject.SetActive(toggle);

        if (!toggle)
        {
            transform.localScale = Vector3.one;
        }
    }

    public void SetDisplayText(string txt)
    {
        DisplayAmountTxt.gameObject.SetActive(true);
        DisplayAmountTxt.text = txt;
    }

    public void SetItemREF(Item item)
    {
        itemREF = item;
    }

    public Item GetItemREF()
    {
        return itemREF;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.97f, 0.97f, 1f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemREF != null)
        {
            if (itemREF.isNew)
            {
                itemREF.isNew = false;
                DisableNewImage();
            }
        }
        //Burst.gameObject.SetActive(true);
        //Burst.Emit(1);
        onButtonClick?.Invoke(this);
        transform.localScale = Vector3.one;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        Outline.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DragnDropButton dragnDrop = GetComponent<DragnDropButton>();
        dragnDrop.onBeginDragEvent -= OnBeginDrag;
        dragnDrop.onDragEvent -= OnDrag;
        dragnDrop.onEndDragEvent -= OnEndDrag;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.02f, 1.02f, 1f);
        Outline.gameObject.SetActive(true);
    }
}
