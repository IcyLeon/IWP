using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : UpgradableItems
{
    private float PreviousHealthRatio;
    private float CurrentHealth;
    private float CurrentEnergyBurstCost;
    private float EnergyBurstCost;
    private float CurrentElementalSkillCooldown;
    private float ElementalSkillCooldown;
    private float CurrentElementalBurstEnergyCooldown;
    private float ElementalBurstEnergyCooldown;
    private int MaxAscensionLevel;
    private int CurrentAscension;
    private ElementalReaction elementalReaction;
    private List<Artifacts> EquippedArtifacts = new List<Artifacts>();
    private List<Food> FoodBuffList = new List<Food>();

    public int GetCurrentAscension()
    {
        return CurrentAscension;
    }
    public List<Artifacts> GetEquippedArtifactsList()
    {
        return EquippedArtifacts;
    }
    public List<Food> GetFoodBuffList()
    {
        return FoodBuffList;
    }

    public void ConsumeFood(Food food)
    {
        Food existFood = CheckIfFoodTypeExist(food);
        if (existFood == null)
        {
            FoodBuffList.Add(food);
        }
        else
        {
            if (existFood is BuffFood BuffFood)
                BuffFood.ResetTimer();
        }
    }

    private Food CheckIfFoodTypeExist(Food food)
    {
        for (int i = 0; i < GetFoodBuffList().Count; i++)
        {
            if (GetFoodBuffList()[i].GetItemSO() == food.GetItemSO())
            {
                return GetFoodBuffList()[i];
            }
        }
        return null;
    }

    public Artifacts CheckIfArtifactTypeExist(ArtifactType artifacttype)
    {
        for (int i = 0; i < EquippedArtifacts.Count; i++)
        {
            if (EquippedArtifacts[i].GetArtifactType() == artifacttype)
            {
                return EquippedArtifacts[i];
            }
        }
        return null;
    }

    public float GetCurrentEnergyBurstCost()
    {
        return CurrentEnergyBurstCost;
    }

    public void AddorRemoveCurrentEnergyBurstCost(float amt)
    {
        CurrentEnergyBurstCost += amt;
    }

    public float GetEnergyBurstCost()
    {
        return EnergyBurstCost;
    }

    private void UpdateFoodList()
    {
        for (int i = FoodBuffList.Count - 1; i >= 0; i--)
        {
            Food food = FoodBuffList[i];
            if (food != null)
            {
                if (food is BuffFood BuffFood)
                {
                    if (BuffFood.isFoodBuffEnds())
                    {
                        FoodBuffList.RemoveAt(i);
                        return;
                    }
                }
                food.Update();
            }
        }
    }
    public CharacterData(bool isNew, PlayerCharacterSO playerCharacterSO) : base(isNew, playerCharacterSO)
    {
        MaxAscensionLevel = CurrentAscension = 0;
        SetItemsSO(playerCharacterSO);
        ResetEnergyCost();
        ElementalSkillCooldown = playerCharacterSO.ElementalSkillsCooldown;
        ElementalBurstEnergyCooldown = playerCharacterSO.UltiSkillCooldown;
        CurrentElementalSkillCooldown = 0;
        CurrentElementalBurstEnergyCooldown = 0;
        EnergyBurstCost = playerCharacterSO.EnergyCost;
        Level = 1;
        CurrentHealth = GetBaseMaxHealth(GetLevel());
        SetPreviousHealthRatio(GetHealth() / GetActualMaxHealth(GetLevel()));
        MaxLevel = 20;
        elementalReaction = new ElementalReaction();
    }

    public CharacterData(bool isNew, PlayerCharacterSO playerCharacterSO, float damage, int level, float currentEnergy) : base(isNew, playerCharacterSO)
    {
        MaxAscensionLevel = CurrentAscension = 0;
        SetItemsSO(playerCharacterSO);
        Level = level;
        CurrentHealth = GetActualMaxHealth(GetLevel());
        SetPreviousHealthRatio(GetHealth() / GetActualMaxHealth(GetLevel()));
        CurrentEnergyBurstCost = currentEnergy;
        ElementalSkillCooldown = playerCharacterSO.ElementalSkillsCooldown;
        ElementalBurstEnergyCooldown = playerCharacterSO.UltiSkillCooldown;
        CurrentElementalSkillCooldown = 0;
        CurrentElementalBurstEnergyCooldown = 0;
        CurrentAscension = 0;
        EnergyBurstCost = playerCharacterSO.EnergyCost;
        MaxLevel = 20;
        elementalReaction = new ElementalReaction();
    }

    public ElementalReaction GetElementalReaction()
    {
        return elementalReaction;
    }
    public PlayerCharacterSO GetPlayerCharacterSO()
    {
        return GetItemSO() as PlayerCharacterSO;
    }

    public void Update()
    {
        CurrentElementalSkillCooldown -= Time.deltaTime;
        CurrentElementalSkillCooldown = Mathf.Clamp(CurrentElementalSkillCooldown, 0f, ElementalSkillCooldown);

        CurrentElementalBurstEnergyCooldown -= Time.deltaTime;
        CurrentElementalBurstEnergyCooldown = Mathf.Clamp(CurrentElementalBurstEnergyCooldown, 0f, ElementalBurstEnergyCooldown);

        CurrentEnergyBurstCost = Mathf.Clamp(CurrentEnergyBurstCost, 0f, EnergyBurstCost);

        if (GetElementalReaction() != null)
            GetElementalReaction().UpdateElementsList();

        UpdateFoodList();

        if (IsDead())
        {
            ResetEnergyCost();
        }
    }

    public void ResetElementalSkillCooldown()
    {
        CurrentElementalSkillCooldown = ElementalSkillCooldown;
    }

    public void ResetElementalBurstCooldown()
    {
        CurrentElementalBurstEnergyCooldown = ElementalBurstEnergyCooldown;
        ResetEnergyCost();
    }

    public void ResetEnergyCost()
    {
        CurrentEnergyBurstCost = 0f;
    }

    public bool CanTriggerESKill()
    {
        return CurrentElementalSkillCooldown <= 0;
    }

    public bool CanTriggerBurstSKill()
    {
        return CurrentElementalBurstEnergyCooldown <= 0;
    }
    public bool CanTriggerBurstSKillCost()
    {
        return CurrentEnergyBurstCost >= EnergyBurstCost;
    }

    public float GetCurrentElementalSkillCooldown()
    {
        return CurrentElementalSkillCooldown;
    }
    public float GetCurrentElementalBurstCooldown()
    {
        return CurrentElementalBurstEnergyCooldown;
    }

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }
    public void SetHealth(float Hp)
    {
        CurrentHealth = (int)Hp;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, GetActualMaxHealth(GetLevel()));
    }

    public void SetPreviousHealthRatio(float Ratio) // just to store ratio
    {
        PreviousHealthRatio = Ratio;
    }

    public float GetPreviousHealthRatio()
    {
        return PreviousHealthRatio;
    }
    public override void Upgrade()
    {
        if (Level != MaxLevel)
        {
            base.Upgrade();
            SetHealth(GetActualMaxHealth(GetLevel()));
        }
    }
    public float GetBaseEM(int level)
    {
        return 0f;
    }

    public float GetActualEM(int level)
    {
        return GetBaseEM(level) + ArtifactsManager.GetInstance().GetTotalFoodAndArtifactValueStatsIncludePercentageAndBaseStats(this, Artifacts.ArtifactsStat.EM);
    }



    public float GetBaseDEF(int level)
    {
        if (level < 0)
            level = Mathf.Abs(level);
        return Mathf.RoundToInt(GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseDEF + ((GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseMaxDEF - GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseDEF) / (GetMaxLevel() - 1)) * (level - 1));
    }

    public float GetActualDEF(int level)
    {
        return GetBaseDEF(level) + ArtifactsManager.GetInstance().GetTotalFoodAndArtifactValueStatsIncludePercentageAndBaseStats(this, Artifacts.ArtifactsStat.DEF);
    }

    public float GetActualATK(int level)
    {
        int CritCoeficient;
        float CritDmgValue = ArtifactsManager.GetInstance().GetTotalFoodAndArtifactValueStatsIncludePercentageAndBaseStats(this, Artifacts.ArtifactsStat.CritDamage);
        float CritRateValue = ArtifactsManager.GetInstance().GetTotalFoodAndArtifactValueStatsIncludePercentageAndBaseStats(this, Artifacts.ArtifactsStat.CritRate);
        CritCoeficient = 1;
        if (!AssetManager.isInProbabilityRange(CritRateValue * 0.01f))
        {
            CritDmgValue = 0f;
            CritCoeficient = 0;
        }

        float atkValue = GetBaseATK(level) + ArtifactsManager.GetInstance().GetTotalFoodAndArtifactValueStatsIncludePercentageAndBaseStats(this, Artifacts.ArtifactsStat.ATK);
        return atkValue + atkValue * (1 + CritDmgValue * 0.01f) * CritCoeficient;
    }

    public int GetBaseMaxHealth(int level)
    {
        if (level < 0)
            level = Mathf.Abs(level);
        return Mathf.RoundToInt(GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseHP + ((GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseMaxHP - GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseHP) / (GetMaxLevel() - 1)) * (level - 1));
    }
    public int GetBaseATK(int level)
    {
        if (level < 0)
            level = Mathf.Abs(level);
        return Mathf.RoundToInt(GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseATK + ((GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseMaxATK - GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseATK) / (GetMaxLevel() - 1)) * (level - 1));
    }
    public float GetActualMaxHealth(int level)
    {
        return GetBaseMaxHealth(level) + ArtifactsManager.GetInstance().GetTotalFoodAndArtifactValueStatsIncludePercentageAndBaseStats(this, Artifacts.ArtifactsStat.HP);
    }

    public override int GetMaxLevel()
    {
        return base.GetMaxLevel() * (MaxAscensionLevel + 1);
    }
}