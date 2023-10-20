using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : UpgradableItems
{
    private float Health;
    private float Damage;
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

    public CharacterData()
    {
        Health = 1;
        Damage = 0;
        Level = 1;
    }

    public CharacterData(float health, float damage, int level)
    {
        Health = health;
        Damage = damage;
        Level = level;
    }
}
