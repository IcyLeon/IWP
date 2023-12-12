using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyKillerData
{
    private float CurrentHealth;
    private float MaxHealth;
    private float DetectionRange;
    private FriendlyKillerSO FriendlyKillerSOREF;

    public FriendlyKillerData(FriendlyKillerSO FriendlyKillerSO)
    {
        FriendlyKillerSOREF = FriendlyKillerSO;
        MaxHealth = GetFriendlyKillerSO().BaseMaxHealth;
        CurrentHealth = MaxHealth;
    }

    public FriendlyKillerSO GetFriendlyKillerSO()
    {
        return FriendlyKillerSOREF;
    }
    public float GetMaxHealth()
    {
        return MaxHealth;
    }
    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }

    public void SetCurrentHealth(float CurrentHealth)
    {
        this.CurrentHealth = CurrentHealth;
    }

    public float GetDetectionRange()
    {
        if (GetFriendlyKillerSO() == null)
            return 1;

        return GetFriendlyKillerSO().DetectionRange;
    }
}