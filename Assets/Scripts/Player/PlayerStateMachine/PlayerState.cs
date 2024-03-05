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

    public Rigidbody rb;

    private PlayerManager PlayerManager;

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
        //Debug.Log(currentState);
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

    public PlayerManager GetPlayerManager()
    {
        return PlayerManager;
    }
    public PlayerState(PlayerManager playerManager)
    {
        PlayerManager = playerManager;
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
    }

    private void OnCharacterChange(CharacterData cd, PlayerCharacters playerCharacters)
    {
        ChangeState(playerIdleState);
    }
}
