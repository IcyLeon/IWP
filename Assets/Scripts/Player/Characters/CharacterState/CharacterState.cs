using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
    protected Characters Characters;

    protected IState currentState;
    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        currentState.Enter();
    }

    public void Update()
    {
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

    public CharacterState(Characters Characters)
    {
        this.Characters = Characters;
    }
}
