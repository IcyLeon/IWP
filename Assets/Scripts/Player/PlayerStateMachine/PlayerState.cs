using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void Update();
    void FixedUpdate();
    void LateUpdate();
    public void OnAnimationTransition();
}

public class PlayerState
{
    private IState currentState;

    public Rigidbody rb;

    private PlayerManager PlayerManager;
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
    public PlayerAttackState playerAttackState { get; }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            PlayerManager.onCharacterChange -= OnCharacterChange;
            currentState.Exit();
        }

        currentState = newState;

        PlayerManager.onCharacterChange += OnCharacterChange;

        currentState.Enter();
    }

    public PlayerMovementState GetPlayerMovementState()
    {
        PlayerMovementState p = currentState as PlayerMovementState;

        return p;
    }
    public void Update()
    {
        if (currentState != null)
            currentState.Update();
    }

    public void FixedUpdate()
    {
        if (currentState != null)
            currentState.FixedUpdate();
    }
    public void LateUpdate()
    {
        if (currentState != null)
            currentState.LateUpdate();
    }

    public void OnAnimationTransition()
    {
        if (currentState != null)
            currentState.OnAnimationTransition();
    }

    public PlayerManager GetPlayerManager()
    {
        return PlayerManager;
    }
    public PlayerController GetPlayerController()
    {
        return PlayerController;
    }
    public PlayerState(PlayerManager playerManager, PlayerController playerController)
    {
        PlayerManager = playerManager;
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

        ChangeState(playerIdleState);
        PlayerController = playerController;
    }

    private void OnCharacterChange(CharacterData cd, PlayerCharacters playerCharacters)
    {
        ChangeState(playerIdleState);
    }
}
