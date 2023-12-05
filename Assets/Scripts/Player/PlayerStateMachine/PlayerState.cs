using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void Update();
    void FixedUpdate();
}

public class PlayerState
{
    private IState currentState;

    private PlayerController PlayerController;

    public PlayerData PlayerData;
    public PlayerStoppingState playerStoppingState { get; }
    public PlayerFallingState playerFallingState { get; }
    public PlayerRunState playerRunState { get; }
    public PlayerIdleState playerIdleState { get; }
    public PlayerJumpState playerJumpState { get; }
    public PlayerPlungeState playerPlungeState { get; }
    public PlayerDashState playerDashState { get; }
    public PlayerSprintState playerSprintState { get; }
    public PlayerAimState playerAimState { get; }
    public PlayerBurstState playerBurstState { get; }

    public PlayerAttackState playerAttackState { get; }
    public delegate void PlayerStateChange(PlayerState state);
    public PlayerStateChange OnPlayerStateChange;

    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        currentState.Enter();
        OnPlayerStateChange?.Invoke(this);
    }

    public PlayerMovementState GetPlayerMovementState()
    {
        PlayerMovementState p = currentState as PlayerMovementState;

        return p;
    }
    public void Update()
    {
        Debug.Log(currentState);
        currentState.Update();
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public PlayerController GetPlayerController()
    {
        return PlayerController;
    }
    public PlayerState(PlayerController playerController)
    {
        PlayerData = new PlayerData();
        playerStoppingState = new PlayerStoppingState(this);
        playerFallingState = new PlayerFallingState(this);
        playerRunState = new PlayerRunState(this);
        playerIdleState = new PlayerIdleState(this);
        playerJumpState = new PlayerJumpState(this);
        playerDashState = new PlayerDashState(this);
        playerSprintState = new PlayerSprintState(this);
        playerPlungeState = new PlayerPlungeState(this);
        playerAimState = new PlayerAimState(this);
        playerBurstState = new PlayerBurstState(this);
        playerAttackState = new PlayerAttackState(this);
        PlayerController = playerController;

        ChangeState(playerIdleState);
    }
}
