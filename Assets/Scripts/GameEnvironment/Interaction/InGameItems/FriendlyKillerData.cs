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
    public float GetHealth()
    {
        return CurrentHealth;
    }

    public void SetHealth(float val)
    {
        CurrentHealth = val;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, GetMaxHealth());
    }

    public float GetDetectionRange()
    {
        if (GetFriendlyKillerSO() == null)
            return 1;

        return GetFriendlyKillerSO().DetectionRange;
    }
}
