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
    private PlayerController PlayerController;
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
    }

    public static StaminaManager GetInstance()
    {
        return instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        WaitElapsed = 0f;
        WaitToRegen = 1.5f;
        staminaData = new StaminaData();
    }

    private void Update()
    {
        UpdateController();
        UpdateRegen();
        UpdateStamina();
    }

    private void UpdateController()
    {
        if (PlayerController != null)
            return;

        PlayerController = CharacterManager.GetInstance().GetPlayerManager().GetPlayerController();

        if (PlayerController == null)
            PlayerController.GetPlayerState().OnPlayerStateChange += PlayerStateChange;
    }

    private void PlayerStateChange(PlayerState state)
    {
        switch (PlayerController.GetPlayerManager().GetPlayerMovementState().GetPlayerStateEnum())
        {
            case PlayerMovementState.PlayerStateEnum.DASH:
                if (GetStaminaSO() != null)
                    PerformStaminaAction(GetStaminaSO().DashCost);                
                break;
        }
    }
    private void OnDestroy()
    {
        if (PlayerController == null)
            return;

        //PlayerController.onPlayerStateChange -= PlayerStateChange;
        PlayerController.GetPlayerState().OnPlayerStateChange -= PlayerStateChange;
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

        if (staminaScript == null)
            staminaScript = MainUI.GetInstance().GetStaminaBar();

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

    //private void PlayerStateChange()
    //{
    //    switch(PlayerController.GetPlayerActionStatus())
    //    {
    //        case PlayerActionStatus.DASH:
    //            if (GetStaminaSO() != null)
    //                PerformStaminaAction(GetStaminaSO().DashCost);
    //            break;
    //    }
    //}
}
