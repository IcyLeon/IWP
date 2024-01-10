using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodGadget : GadgetConsumableItem
{
    private CharacterManager CM;
    private Food CurrentFood;

    public FoodGadget(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        CM = CharacterManager.GetInstance();
    }

    public Food GetCurrentFood()
    {
        return CurrentFood;
    }
    private void SetCurrentFood(Food food)
    {
        CurrentFood = food;
        UpdateContent();
    }

    public override void Update()
    {
        CharacterData DeadCharacterData = GetDeadCharacterData();
        CharacterData CurrentCharacterData = GetCurrentCharacterData();

        Food food;
        if (DeadCharacterData != null)
        {
            food = GetFirstFood<ReviveFood>();
            if (food != null)
                food.SetCharacterData(DeadCharacterData);
        }
        else
        {
            food = GetFirstFood<Food>();
            if (food != null)
                food.SetCharacterData(CurrentCharacterData);
        }
        SetCurrentFood(food);
    }

    private Food GetFirstFood<T>()
    {
        List<Food> foodList = InventoryManager.GetInstance().GetItemListOfType<Food>();

        for (int i = foodList.Count - 1; i >= 0; i--)
        {
            Food food = foodList[i];

            if (food is not T)
            {
                Debug.Log($"Removing item because it's not of type {typeof(T)}");
                foodList.RemoveAt(i);
            }
            else if (food is ReviveFood && (typeof(T) == typeof(Food) ||
                food.GetFoodData().GetFoodType() != FoodData.FoodType.RESTORE_HEALTH))
            {
                Debug.Log($"Removing item because it's not a ReviveFoodSO and is FoodData with non-Restore Health type");
                foodList.RemoveAt(i);
            }
        }

        if (foodList.Count == 0)
            return null;

        foodList = foodList.OrderBy(item => (int)item.GetRarity()).ToList();

        return foodList[0];
    }

    private CharacterData GetDeadCharacterData()
    {
        return CM.GetPlayerManager().GetDeadCharacter();
    }
    private CharacterData GetCurrentCharacterData()
    {
        if (CM.GetPlayerManager().GetCurrentCharacter() == null)
            return null;

        return CM.GetPlayerManager().GetCurrentCharacter().GetCharacterData();
    }

    protected override void Use(int val)
    {
        if (CurrentFood != null)
        {
            CurrentFood.Use(val);
            UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (CurrentFood == null)
            return;

        amount = CurrentFood.GetAmount();
    }
}
