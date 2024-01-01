using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyStateMachine : CharacterState
{
    public MageEnemyData MageEnemyData;
    public MageEnemyIdleState MageEnemyIdleState { get; }
    public MageEnemyChaseState MageEnemyChaseState { get; }
    public MageEnemyStunState MageEnemyStunState { get; }
    public MageEnemyShootLaserState MageEnemyShootLaserState { get; }
    public MageEnemySpawnMinionState MageEnemySpawnMinionState { get; }
    public MageEnemyShieldState MageEnemyShieldState { get; }
    public MageEnemy GetMageEnemy()
    {
        return (MageEnemy)Characters;
    }
    public MageEnemyStateMachine(Characters characters) : base(characters)
    {
        MageEnemyData = new MageEnemyData();
        MageEnemyShootLaserState = new MageEnemyShootLaserState(this);
        MageEnemyIdleState = new MageEnemyIdleState(this);
        MageEnemyChaseState = new MageEnemyChaseState(this);
        MageEnemySpawnMinionState = new MageEnemySpawnMinionState(this);
        MageEnemyStunState = new MageEnemyStunState(this);
        MageEnemyShieldState = new MageEnemyShieldState(this);
        ChangeState(MageEnemyIdleState);
    }
}
