using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage {
    void TakeDamage(float damageAmt);
}

public class Characters : MonoBehaviour, IDamage
{
    protected float Health;
    protected float MaxHealth;
    protected int Level;
    protected HealthBarScript healthBarScript;

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.SetupMinAndMax(0, GetMaxHealth());
            healthBarScript.UpdateContent(GetHealth(), GetLevel());
        }
    }
    public void TakeDamage(float amt)
    {
    }

    public virtual float GetHealth()
    {
        return Health;
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
