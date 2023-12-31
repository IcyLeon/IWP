using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : UpgradableItems
{
    private float CurrentHealth;
    private float CurrentEnergyBurstCost;
    private float EnergyBurstCost;
    private float CurrentElementalSkillCooldown;
    private float ElementalSkillCooldown;
    private float CurrentElementalBurstEnergyCooldown;
    private float ElementalBurstEnergyCooldown;
    private int MaxAscensionLevel;
    private int PreviewsMaxCapLevel;
    private int CurrentAscension;
    private ElementalReaction elementalReaction;
    private List<Artifacts> EquippedArtifacts = new List<Artifacts>();

    public int GetCurrentAscension()
    {
        return CurrentAscension;
    }
    public List<Artifacts> GetEquippedArtifactsList()
    {
        return EquippedArtifacts;
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

    public CharacterData(bool isNew, PlayerCharacterSO playerCharacterSO) : base(isNew, playerCharacterSO)
    {
        MaxAscensionLevel = CurrentAscension = PreviewsMaxCapLevel = 0;
        SetItemsSO(playerCharacterSO);
        ResetEnergyCost();
        ElementalSkillCooldown = playerCharacterSO.ElementalSkillsCooldown;
        ElementalBurstEnergyCooldown = playerCharacterSO.UltiSkillCooldown;
        CurrentElementalSkillCooldown = 0;
        CurrentElementalBurstEnergyCooldown = 0;
        EnergyBurstCost = playerCharacterSO.EnergyCost;
        Level = 1;
        CurrentHealth = GetMaxHealth(GetLevel());
        MaxLevel = 20;
        elementalReaction = new ElementalReaction();
    }

    public CharacterData(bool isNew, PlayerCharacterSO playerCharacterSO, float damage, int level, float currentEnergy) : base(isNew, playerCharacterSO)
    {
        MaxAscensionLevel = CurrentAscension = 0;
        SetItemsSO(playerCharacterSO);
        Level = level;
        CurrentHealth = GetMaxHealth(GetLevel());
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
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, GetMaxHealth(GetLevel()));
    }

    public override void Upgrade()
    {
        if (Level != MaxLevel)
        {
            base.Upgrade();
            SetHealth(GetMaxHealth(GetLevel()));
        }
    }
    public float GetEM(int level)
    {
        return 0f;
    }

    public float GetDEF(int level)
    {
        if (level < 0)
            level = Mathf.Abs(level);
        return Mathf.RoundToInt(GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseDEF + ((GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseMaxDEF - GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseDEF) / (GetMaxLevel() - 1)) * (level - 1));
    }

    public int GetATK(int level)
    {
        if (level < 0)
            level = Mathf.Abs(level);
        return Mathf.RoundToInt(GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseATK + ((GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseMaxATK - GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseATK) / (GetMaxLevel() - 1)) * (level - 1));
    }

    public int GetMaxHealth(int level)
    {
        if (level < 0)
            level = Mathf.Abs(level);
        return Mathf.RoundToInt(GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseHP + ((GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseMaxHP - GetPlayerCharacterSO().GetAscensionInfo(GetCurrentAscension()).BaseHP) / (GetMaxLevel() - 1)) * (level - 1));
    }

    public override int GetMaxLevel()
    {
        return base.GetMaxLevel() * (MaxAscensionLevel + 1);
    }
}