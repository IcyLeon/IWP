using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage {
    void TakeDamage(Vector3 position, Elements elements, float damageAmt);
}

public class Characters : MonoBehaviour, IDamage
{
    protected float CurrentHealth;
    protected float MaxHealth;
    protected int Level;
    protected HealthBarScript healthBarScript;
    private ElementalReactionManager elementalReactionManager;

    protected virtual void Start()
    {
        elementalReactionManager = new ElementalReactionManager();
    }

    public void PrintReaction()
    {
        for (int i = 0; i < elementalReactionManager.GetElementsList().Count; i++)
        {
            Elements e = elementalReactionManager.GetElementsList()[i];
        }
        Debug.Log(elementalReactionManager.GetElementsList().Count);
    }

    protected virtual void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.SetupMinAndMax(0, GetMaxHealth());
            healthBarScript.UpdateContent(GetHealth(), GetLevel());
        }

        if (elementalReactionManager != null)
            elementalReactionManager.UpdateElementsList();
    }

    public void TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        Elemental elemental;
        if (elements != null)
            elemental = elements.GetElements();
        else
            elemental = Elemental.NONE;

        if (elementalReactionManager != null)
            elementalReactionManager.AddElements(elements);

        AssetManager.GetInstance().SpawnWorldText(pos, elemental, amt.ToString());
    }

    public virtual float GetHealth()
    {
        return CurrentHealth;
    }

    public virtual float GetMaxHealth()
    {
        return MaxHealth;
    }

    public virtual int GetLevel()
    {
        return Level;
    }
}
