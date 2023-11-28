using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaData
{
    private float MaxStamina;
    private float CurrentStamina;

    public float GetMaxStamina()
    {
        return MaxStamina;
    }

    public void SetMaxStamina(float max)
    {
        MaxStamina = max;
    }


    public float GetCurrentStamina()
    {
        return CurrentStamina;
    }

    public void AddStamina(float val)
    {
        CurrentStamina += val;
    }

    public void ConsumeStamina(float val)
    {
        CurrentStamina -= val;
    }

    public void Update()
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, GetMaxStamina());
    }
    public StaminaData()
    {
        SetMaxStamina(100f);
        CurrentStamina = MaxStamina;
    }
}
