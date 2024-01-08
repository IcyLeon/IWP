using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBot : BaseEnemy
{
    private PunchingBotStateMachine m_StateMachine;
    [SerializeField] PunchingBotPunchCollider m_PunchCollider;

    // Start is called before the first frame update
    protected override void Start()
    {
        m_StateMachine = new PunchingBotStateMachine(this);
        base.Start();
    }

    private void WackPlayer()
    {
        m_PunchCollider.WackPlayer();
    }

    // Update is called once per frame
    protected override void Update()
    {
        m_StateMachine.Update();
        base.Update();
        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
    }

    public override bool UpdateDie()
    {
        bool isdead = base.UpdateDie();
        if (isdead)
        {
            DisableAgent();
            if (DieCoroutine == null)
            {
                DieCoroutine = StartCoroutine(Disappear());
            }
        }
        return isdead;
    }

    private void TriggerOnPunchingBotStateAnimationTransition()
    {
        m_StateMachine.OnAnimationTransition();
    }

    private void FixedUpdate()
    {
        m_StateMachine.FixedUpdate();
    }
}
