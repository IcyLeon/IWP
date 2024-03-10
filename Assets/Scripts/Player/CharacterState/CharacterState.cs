using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
    protected Characters Characters;

    protected IState currentState;

    public IState GetCurrentState()
    {
        return currentState;
    }
    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        if (newState != null)
            currentState.Enter();
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

    public CharacterState(Characters Characters)
    {
        this.Characters = Characters;
    }
}
