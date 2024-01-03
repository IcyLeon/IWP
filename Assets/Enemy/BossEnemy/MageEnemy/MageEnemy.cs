using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemy : BaseEnemy
{
    private MageEnemyStateMachine m_StateMachine;
    private float FireHitInterval = 0.15f, FireHitIntervalElapsed;
    [Header("Fire")]
    [SerializeField] CapsuleCollider FireBreathingCollider;
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

    private void TurnOnFireBreathingCollider()
    {
        GetFireBreathingCollider().enabled = true;
        FireHitIntervalElapsed = Time.time;
    }
    public void TurnOFFireBreathingCollider()
    {
        GetFireBreathingCollider().enabled = false;
        FireHitIntervalElapsed = Time.time;
    }
    private void UpdateFireBreathing()
    {
        if (!GetFireBreathingCollider().enabled)
            return;

        Collider[] colliders = Physics.OverlapCapsule(GetFireBreathingCollider().bounds.min,
                                                      GetFireBreathingCollider().bounds.max,
                                                      GetFireBreathingCollider().radius,
                                                      LayerMask.GetMask("Player"));

        foreach (Collider other in colliders)
        {
            PlayerCharacters PlayerCharacters = other.GetComponent<PlayerCharacters>();
            if (PlayerCharacters != null)
            {
                if (!PlayerCharacters.GetPlayerManager().IsSkillCasting())
                {
                    if (!PlayerCharacters.IsDead() && Time.time - FireHitIntervalElapsed > FireHitInterval)
                    {
                        PlayerCharacters.TakeDamage(PlayerCharacters.GetPointOfContact(), new Elements(Elemental.FIRE), 100f);
                        FireHitIntervalElapsed = Time.time;
                    }
                }
            }
        }
    }

    private void SpawnEnemies()
    {
        BossManager BM = BossManager.GetInstance();

        if (BM == null)
            return;

        for (int i = 0; i < m_StateMachine.MageEnemyData.SpawnEnemiesAmount; i++)
        {
            int randomIndex = Random.Range(0, EM.GetEnemyInfosList().Length);
            EnemyType ET = (EnemyType)randomIndex;
            BM.SpawnGroundUnitsWithinTerrain(ET);
        }
    }

    public CapsuleCollider GetFireBreathingCollider()
    {
        return FireBreathingCollider;
    }
    private void TriggerOnMageStateAnimationTransition()
    {
        m_StateMachine.OnAnimationTransition();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        TurnOFFireBreathingCollider();
        m_StateMachine = new MageEnemyStateMachine(this);
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        m_StateMachine.Update();
        base.Update();
        Debug.Log(m_StateMachine.GetCurrentState());
        UpdateFireBreathing();
        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
    }

    protected override void UpdateState()
    {
        if (NavMeshAgent == null)
            return;

        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
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
