using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void Update();
    void FixedUpdate();
    public void OnAnimationTransition();
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
    public PlayerDeadState playerDeadState { get; }
    public PlayerLandingState playerLandingState { get; }

    public PlayerStayAirborneState playerStayAirborneState { get; }
    public PlayerAttackState playerAttackState { get; }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            GetPlayerController().GetPlayerManager().onCharacterChange -= OnCharacterChange;
            currentState.Exit();
        }

        currentState = newState;

        GetPlayerController().GetPlayerManager().onCharacterChange += OnCharacterChange;

        currentState.Enter();
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

    public void OnAnimationTransition()
    {
        currentState.OnAnimationTransition();
    }

    public PlayerController GetPlayerController()
    {
        return PlayerController;
    }
    public PlayerState(PlayerController playerController)
    {
        PlayerController = playerController;
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
        playerDeadState = new PlayerDeadState(this);
        playerLandingState = new PlayerLandingState(this);
        playerStayAirborneState = new PlayerStayAirborneState(this);

        ChangeState(playerIdleState);
    }

    private void OnCharacterChange(CharacterData cd)
    {
        ChangeState(playerIdleState);
    }
}
