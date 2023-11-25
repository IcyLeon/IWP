using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaminaManager : MonoBehaviour
{
    private float MaxStamina, CurrentStamina;
    private float WaitToRegen;
    private float WaitElapsed;
    private StaminaScript staminaScript;
    public event Action OnDash;
    private PlayerController PlayerController;
    [SerializeField] StaminaSO StaminaSO;

    // Start is called before the first frame update
    void Start()
    {
        MaxStamina = 100f;
        CurrentStamina = 0;
        WaitElapsed = 0f;
        WaitToRegen = 1.5f;
        staminaScript = MainUI.GetInstance().GetStaminaBar();
        PlayerController = GetComponent<PlayerController>();
    }


    private void Update()
    {
        UpdateRegen();
        UpdateStamina();
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, MaxStamina);
    }

    private void UpdateRegen()
    {
        if (GetStaminaSO() == null)
            return;

        if (WaitElapsed > WaitToRegen)
            CurrentStamina += Time.deltaTime * GetStaminaSO().RegenStaminaPerSec;

        WaitElapsed += Time.deltaTime;
        WaitElapsed = Mathf.Clamp(WaitElapsed, 0f, WaitToRegen + 1f);
    }

    private void ResetCounter()
    {
        WaitElapsed = 0;
    }

    private void UpdateStamina()
    {
        if (staminaScript == null)
            return;

        staminaScript.UpdateStamina(CurrentStamina / MaxStamina);
    }

    public bool PerformStaminaAction(float Cost)
    {
        if (CanPerformStaminaAction(Cost))
        {
            CurrentStamina -= Cost;
            ResetCounter();
            return true;
        }
        return false;
    }

    private bool CanPerformStaminaAction(float Cost)
    {
        return CurrentStamina > Cost;
    }

    public StaminaSO GetStaminaSO()
    {
        return StaminaSO;
    }

    public void PerformDash()
    {
        if (GetStaminaSO() == null)
            return;

        float Cost = GetStaminaSO().DashCost;
        if (PerformStaminaAction(Cost))
        {
            OnDash?.Invoke();
        }

    }
}
