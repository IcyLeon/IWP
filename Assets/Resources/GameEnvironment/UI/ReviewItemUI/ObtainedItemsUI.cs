using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ObtainedItemsUI : MonoBehaviour
{
    [SerializeField] Transform ItemQualityParentTransform;
    [SerializeField] Button CloseButton;
    [SerializeField] ItemContentManager ItemContentManager;
    [SerializeField] GameObject ItemContentInfo;
    private List<ItemButton> ItemButtonList;

    // Start is called before the first frame update
    void Awake()
    {
        ItemButtonList = new();
        InventoryManager.OnItemGiveToPlayer += OnItemGiveToPlayer;
    }
    private void Start()
    {
        MainUI.GetInstance().SetPaused(true);
        CloseButton.onClick.AddListener(ClosePanel);
    }

    private void OnDestroy()
    {
        for (int i = ItemButtonList.Count - 1; i >= 0; i--)
        {
            ItemButton itemButton = ItemButtonList[i];
            itemButton.onButtonSpawn -= OnItemSpawned;
            itemButton.onButtonClick -= GetItemSelected;
        }

        InventoryManager.OnItemGiveToPlayer -= OnItemGiveToPlayer;
        MainUI.GetInstance().SetPaused(false);
    }
    private void ClosePanel()
    {
        Destroy(gameObject);
    }

    private void OnItemGiveToPlayer(List<Item> ItemList)
    {
        InventoryManager IM = InventoryManager.GetInstance();
        for (int i = 0; i < ItemList.Count; i++)
        {
            Item item = ItemList[i];
            IM.AddItems(item);

            ItemButton itemButton = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, ItemQualityParentTransform).GetComponent<ItemButton>();
            itemButton.SetItemsSO(item.GetItemSO());
            itemButton.SetItemREF(item);
            itemButton.onButtonSpawn += OnItemSpawned;
            itemButton.onButtonClick += GetItemSelected;
            ItemButtonList.Add(itemButton);
        }
    }
    private void OnItemSpawned(ItemButton itemButton)
    {
        itemButton.DisableNewImage();
    }
    private void GetItemSelected(ItemButton itemButton)
    {
        ItemContentManager.SetItemREF(itemButton.GetItemREF(), itemButton.GetItemsSO());
        ItemContentManager.DisableLockButton();
        ItemContentInfo.SetActive(true);
    }
}
