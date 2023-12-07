using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : UpgradableItems
{
    private float CurrentHealth;
    private float BaseMaxHealth;
    private float BaseATK;
    private float BaseDEF;
    private float CurrentEnergyBurstCost;
    private float EnergyBurstCost;
    private float CurrentElementalSkillCooldown;
    private float ElementalSkillCooldown;
    private float CurrentElementalBurstEnergyCooldown;
    private float ElementalBurstEnergyCooldown;
    private ElementalReaction elementalReaction;
    private List<Artifacts> EquippedArtifacts = new List<Artifacts>();

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
        SetItemsSO(playerCharacterSO);
        BaseMaxHealth = playerCharacterSO.BaseHP;
        CurrentHealth = BaseMaxHealth;
        BaseATK = playerCharacterSO.BaseATK;
        CurrentEnergyBurstCost = 0;
        ElementalSkillCooldown = playerCharacterSO.ElementalSkillsCooldown;
        ElementalBurstEnergyCooldown = playerCharacterSO.UltiSkillCooldown;
        CurrentElementalSkillCooldown = 0;
        CurrentElementalBurstEnergyCooldown = 0;
        EnergyBurstCost = playerCharacterSO.EnergyCost;
        Level = 1;
        MaxLevel = 20;
        elementalReaction = new ElementalReaction();
    }

    public CharacterData(bool isNew, PlayerCharacterSO playerCharacterSO, float damage, int level, float currentEnergy) : base(isNew, playerCharacterSO)
    {
        SetItemsSO(playerCharacterSO);
        BaseMaxHealth = playerCharacterSO.BaseHP;
        CurrentHealth = BaseMaxHealth;
        BaseATK = damage;
        Level = level;
        CurrentEnergyBurstCost = currentEnergy;
        ElementalSkillCooldown = playerCharacterSO.ElementalSkillsCooldown;
        ElementalBurstEnergyCooldown = playerCharacterSO.UltiSkillCooldown;
        CurrentElementalSkillCooldown = 0;
        CurrentElementalBurstEnergyCooldown = 0;
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
            CurrentEnergyBurstCost = 0f;
        }
    }

    public void ResetElementalSkillCooldown()
    {
        CurrentElementalSkillCooldown = ElementalSkillCooldown;
    }

    public void ResetElementalBurstCooldown()
    {
        CurrentElementalBurstEnergyCooldown = ElementalBurstEnergyCooldown;
        CurrentEnergyBurstCost = 0;
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
        CurrentHealth = Hp;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, GetMaxHealth());
    }

    public override void Upgrade()
    {
        if (Level != MaxLevel)
        {
            base.Upgrade();
            SetHealth(GetMaxHealth());
        }
    }

    public float GetMaxHealth()
    {
        return BaseMaxHealth + (BaseMaxHealth * (Level - 1));
    }

    public float GetDamage()
    {
        return BaseATK * (1 + 0);
    }

    public float GetDEF()
    {
        return BaseDEF * (1 + 0);
    }
}
