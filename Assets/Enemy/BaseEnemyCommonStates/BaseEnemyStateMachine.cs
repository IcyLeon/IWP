using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyStateMachine : CharacterState
{
    public BaseEnemy GetBaseEnemy()
    {
        return (BaseEnemy)Characters;
    }
    public BaseEnemyStateMachine(Characters Characters) : base(Characters)
    {
    }
}
