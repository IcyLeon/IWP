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
    private float CurrentElementalEnergyCooldown;
    private float ElementalEnergyCooldown;
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

    public CharacterData(PlayerCharacterSO playerCharacterSO) : base()
    {
        SetItemsSO(playerCharacterSO);
        BaseMaxHealth = playerCharacterSO.BaseHP;
        CurrentHealth = BaseMaxHealth;
        BaseATK = playerCharacterSO.BaseATK;
        CurrentEnergyBurstCost = 0;
        ElementalEnergyCooldown = playerCharacterSO.ElementalSkillsCooldown;
        CurrentElementalEnergyCooldown = 0;
        EnergyBurstCost = playerCharacterSO.EnergyCost;
        Level = 1;
        MaxLevel = 20;
    }

    public CharacterData(PlayerCharacterSO playerCharacterSO, float damage, int level, float currentEnergy)
    {
        SetItemsSO(playerCharacterSO);
        BaseMaxHealth = playerCharacterSO.BaseHP;
        CurrentHealth = BaseMaxHealth;
        BaseATK = damage;
        Level = level;
        CurrentEnergyBurstCost = currentEnergy;
        ElementalEnergyCooldown = playerCharacterSO.ElementalSkillsCooldown;
        CurrentElementalEnergyCooldown = 0;
        EnergyBurstCost = playerCharacterSO.EnergyCost;
        MaxLevel = 20;
    }

    public void UpdateEnergyCooldown()
    {
        CurrentElementalEnergyCooldown -= Time.deltaTime;
        CurrentElementalEnergyCooldown = Mathf.Clamp(CurrentElementalEnergyCooldown, 0f, ElementalEnergyCooldown);
    }

    public void ResetEnergyCooldown()
    {
        CurrentElementalEnergyCooldown = ElementalEnergyCooldown;
    }

    public bool CanTriggerSKill()
    {
        return CurrentElementalEnergyCooldown <= 0;
    }

    public float GetCurrentElementalSkillCooldown()
    {
        return CurrentElementalEnergyCooldown;
    }

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public void SetHealth(float Hp)
    {
        CurrentHealth = Hp;
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
