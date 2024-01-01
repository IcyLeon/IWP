using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemy : BaseEnemy
{
    private float OriginalColliderHeight;
    private MageEnemyStateMachine m_StateMachine;

    public CapsuleCollider GetCapsuleCollider()
    {
        return (CapsuleCollider)col;
    }

    public float GetOriginalColliderHeight()
    {
        return OriginalColliderHeight;
    }

    private void TriggerOnMageStateAnimationTransition()
    {
        m_StateMachine.OnAnimationTransition();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        m_StateMachine = new MageEnemyStateMachine(this);
        OriginalColliderHeight = GetCapsuleCollider().height;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        m_StateMachine.Update();
        base.Update();
    }

    public override float GetElementalShield()
    {
        return 5000f;
    }

    private void FixedUpdate()
    {
        m_StateMachine.FixedUpdate();
    }
}
