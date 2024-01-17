using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradableItems : Item
{
    protected int Level, MaxLevel;
    protected bool locked;
    protected float CurrentExpAmount;
    protected float TotalExpAmount;
    public event Action onLevelChanged;

    public virtual void Upgrade()
    {
        Level++;
        Level = Mathf.Clamp(Level, 0, MaxLevel);
    }

    public void CallOnLevelChanged()
    {
        onLevelChanged?.Invoke();
    }

    public int GetLevel()
    {
        return Level;
    }

    public virtual int GetMaxLevel()
    {
        return MaxLevel;
    }


    public float GetExpAmount()
    {
        return CurrentExpAmount;
    }

    public void SetExpAmount(float amt)
    {
        CurrentExpAmount = amt;
    }
    public void SetMaxExpAmount(float amt)
    {
        TotalExpAmount = amt;
    }
    public float GetMaxExpAmount()
    {
        return TotalExpAmount;
    }


    public UpgradableItems(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        MaxLevel = 20;
        Level = 0;
        CurrentExpAmount = TotalExpAmount = 0;
        locked = false;
    }

    public void SetLockStatus(bool lockstatus)
    {
        locked = lockstatus;
    }

    public bool GetLockStatus()
    {
        return locked;
    }
}
