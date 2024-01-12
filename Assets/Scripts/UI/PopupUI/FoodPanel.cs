using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodPanel : MessagePanel
{
    private InventoryManager inventoryManager;
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] GameObject AmountToEatContent;
    private Food FoodREF;
    private int AmountToEat = 1;
    private Dictionary<ItemButton, CharacterData> itembutton_Dictionary;
    private ItemButton SelectedItemButton;

    [Header("Amount Use Manager")]
    [SerializeField] TextMeshProUGUI AmountAddTxt;
    [SerializeField] Button DecreaseBtn;
    [SerializeField] Button AddBtn;

    private void Init()
    {
        for (int i = 0; i < inventoryManager.GetCharactersOwnedList().Count; i++)
        {
            CharacterData characterData = inventoryManager.GetCharactersOwnedList()[i];
            ItemButton itemButton = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, ScrollRect.content.transform).GetComponent<ItemButton>();
            itemButton.SetItemsSO(characterData.GetItemSO());
            itemButton.SetItemREF(characterData);
            itemButton.onButtonClick += GetItemSelected;
            itemButton.onButtonSpawn += OnButtonSpawn;
            itembutton_Dictionary.Add(itemButton, characterData);
        }
    }

    private void AddandRemove(int value)
    {
        if (FoodREF == null || SelectedItemButton == null)
            return;

        if (FoodREF is not ReviveFood)
            AmountToEat += value;
        AmountToEat = Mathf.Clamp(AmountToEat, 1, FoodREF.GetAmount());

        UpdateAmountAddTxt();
    }

    private void UpdateAmountAddTxt()
    {
        if (FoodREF == null || SelectedItemButton == null)
            return;

        AmountAddTxt.text = AmountToEat.ToString();
    }


    private void UpdateOutlineSelection()
    {
        if (SelectedItemButton == null)
            return;

        foreach (ItemButton itemButton in itembutton_Dictionary.Keys)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, SelectedItemButton);
    }

    public void SetFoodREF(Food Food)
    {
        FoodREF = Food;
    }

    private void OnButtonSpawn(ItemButton itemButton)
    {
        itemButton.DisableNewImage();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        itembutton_Dictionary = new();
        inventoryManager = InventoryManager.GetInstance();
        Init();

        AddBtn.onClick.AddListener(
            () => AddandRemove(1)
        );
        DecreaseBtn.onClick.AddListener(
            () => AddandRemove(-1)
        );

        if (itembutton_Dictionary.Count > 0) {
            KeyValuePair<ItemButton, CharacterData> itemPair = itembutton_Dictionary.ElementAt(0);
            GetItemSelected(itemPair.Key);
        }
    }

    protected override void ConfirmEvent()
    {
        if (FoodREF == null)
        {
            Close();
            return;
        }

        if (FoodREF.GetAmount() > 0)
        {
            FoodREF.Use(AmountToEat);
            if (FoodREF.GetAmount() == 0)
            {
                Close();
            }
            AddandRemove(0); // just to update the amountoeat var
        }
    }

    private void GetItemSelected(ItemButton itemButton)
    {
        SelectedItemButton = itemButton;

        if (FoodREF != null)
        {
            if (itembutton_Dictionary.TryGetValue(SelectedItemButton, out CharacterData CharacterData))
            {
                SetMessage(CharacterData.GetItemSO().ItemName);
                FoodREF.SetCharacterData(CharacterData);
            }
        }
        UpdateOutlineSelection();
    }


    private void OnDestroy()
    {
        for (int i = itembutton_Dictionary.Count - 1; i >= 0; i--)
        {
            KeyValuePair<ItemButton, CharacterData> itemPair = itembutton_Dictionary.ElementAt(i);
            if (itembutton_Dictionary.TryGetValue(itemPair.Key, out CharacterData itemButton))
            {
                itemPair.Key.onButtonClick -= GetItemSelected;
                itemPair.Key.onButtonSpawn -= OnButtonSpawn;
                itembutton_Dictionary.Remove(itemPair.Key);
                Destroy(itemPair.Key.gameObject);
            }
        }
    }
}
