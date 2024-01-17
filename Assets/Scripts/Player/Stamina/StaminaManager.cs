using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaminaManager : MonoBehaviour
{
    private static StaminaManager instance;
    private StaminaData staminaData;
    private float WaitToRegen;
    private float WaitElapsed;
    private StaminaScript staminaScript;
    private PlayerManager PlayerManager;
    [SerializeField] StaminaSO StaminaSO;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        WaitElapsed = 0f;
        WaitToRegen = 1.5f;
        staminaData = new StaminaData();
    }

    public static StaminaManager GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        UpdateRegen();
        UpdateStamina();
    }

    public void PerformStaminaAction()
    {
        PerformStaminaAction(GetStaminaSO().DashCost);                
    }

    private void UpdateRegen()
    {
        if (GetStaminaSO() == null)
            return;

        if (WaitElapsed > WaitToRegen)
        {
            staminaData.AddStamina(Time.deltaTime * GetStaminaSO().RegenStaminaPerSec);
        }

        WaitElapsed += Time.deltaTime;
        WaitElapsed = Mathf.Clamp(WaitElapsed, 0f, WaitToRegen + 1f);
    }

    private void ResetCounter()
    {
        WaitElapsed = 0;
    }

    private void UpdateStamina()
    {
        if (staminaData != null)
            staminaData.Update();

        if (PlayerManager == null)
            PlayerManager = CharacterManager.GetInstance().GetPlayerManager();

        if (staminaScript == null)
            staminaScript = PlayerManager.GetPlayerCanvasUI().GetStaminaBar();
        else
            staminaScript.UpdateStamina(staminaData.GetCurrentStamina() / staminaData.GetMaxStamina());
    }

    public void PerformStaminaAction(float Cost)
    {
        if (CanPerformStaminaAction(Cost))
        {
            staminaData.ConsumeStamina(Cost);
            ResetCounter();
        }
    }

    public bool CanPerformStaminaAction(float Cost)
    {
        return staminaData.GetCurrentStamina() > Cost;
    }

    public StaminaSO GetStaminaSO()
    {
        return StaminaSO;
    }

    public bool CanPerformDash()
    {
        if (GetStaminaSO() == null)
            return false;

        return CanPerformStaminaAction(GetStaminaSO().DashCost);
    }
}
