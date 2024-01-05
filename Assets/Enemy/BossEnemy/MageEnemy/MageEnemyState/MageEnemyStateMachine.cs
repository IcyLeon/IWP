using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyStateMachine : CharacterState
{
    public MageEnemyData MageEnemyData;
    public MageEnemyIdleState MageEnemyIdleState { get; }
    public MageEnemyChaseState MageEnemyChaseState { get; }
    public MageEnemyStunState MageEnemyStunState { get; }
    public MageEnemyShootFireState MageEnemyShootFireState { get; }
    public MageEnemySpawnMinionState MageEnemySpawnMinionState { get; }
    public MageEnemyShieldState MageEnemyShieldState { get; }
    public MageEnemyDeadState MageEnemyDeadState { get; }
    public MageEnemyBasicAttackState MageEnemyBasicAttackState { get; }
    public MageEnemyChargeAttackState MageEnemyChargeAttackState { get; }
    public MageEnemySpawnCrystalsState MageEnemySpawnCrystalsState { get; }
    public MageEnemyAirborneIdleState MageEnemyAirborneIdleState { get; }
    public MageEnemyTakeOffState MageEnemyTakeOffState { get; }
    public MageEnemyAirborneFireballAttackState MageEnemyAirborneFireballAttackState { get; }
    public MageEnemy GetMageEnemy()
    {
        return (MageEnemy)Characters;
    }
    public MageEnemyStateMachine(Characters characters) : base(characters)
    {
        MageEnemyData = new MageEnemyData();
        MageEnemyShootFireState = new MageEnemyShootFireState(this);
        MageEnemyIdleState = new MageEnemyIdleState(this);
        MageEnemyChaseState = new MageEnemyChaseState(this);
        MageEnemySpawnMinionState = new MageEnemySpawnMinionState(this);
        MageEnemyStunState = new MageEnemyStunState(this);
        MageEnemyShieldState = new MageEnemyShieldState(this);
        MageEnemyDeadState = new MageEnemyDeadState(this);
        MageEnemyBasicAttackState = new MageEnemyBasicAttackState(this);
        MageEnemyChargeAttackState = new MageEnemyChargeAttackState(this);
        MageEnemySpawnCrystalsState = new MageEnemySpawnCrystalsState(this);
        MageEnemyTakeOffState = new MageEnemyTakeOffState(this);
        MageEnemyAirborneIdleState = new MageEnemyAirborneIdleState(this);
        MageEnemyAirborneFireballAttackState = new MageEnemyAirborneFireballAttackState(this);
        ChangeState(MageEnemyIdleState);
    }
}
