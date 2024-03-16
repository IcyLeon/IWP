using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spitter : BaseEnemy
{
    public enum States
    {
        PATROL,
        CHASE,
        STRAFE,
        BASICATK,
        DEAD
    }
    private States state;
    private float StrafeElasped;
    private float DefaultStrafeT = 0.15f, CurrentStrafeT;
    private Coroutine StrafeCoroutine;
    private Coroutine PatrolCoroutine;
    [SerializeField] protected GameObject FireBallPrefab;
    [SerializeField] protected Transform FireEmitter;
    [SerializeField] protected Elemental elemental;
    protected float DefaultAttackRate = 2.5f, CurrentAttackRate;
    private float DefaultBasicAttackRate = 0.8f, CurrentBasicAttackRate;
    private float AttackElapsed;
    private float BasicAttackElapsed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state = States.CHASE;
        DetectionRange = 8f;
        CurrentStrafeT = DefaultStrafeT;
        CurrentBasicAttackRate = DefaultBasicAttackRate;
        Staggering = 10;
        CurrentAttackRate = DefaultAttackRate;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateState();
    }

    protected override void HitEvent(ElementalReactionsTrigger ER, Elements e, IDamage dmg)
    {
        int currentStag = 0;
        base.HitEvent(ER, e, dmg);

        if (e != null)
            currentStag += 2;
        if (ER != null)
            currentStag += 4;

        TriggerStaggering(currentStag);
    }

    protected override void UpdateState()
    {
        if (NavMeshAgent == null)
            return;

        if (IsDead())
            return;


        switch (state)
        {
            case States.PATROL:
                if (PatrolCoroutine == null)
                {
                    SetRandomDestination();
                    NavMeshAgent.SetDestination(RamdomDestination);
                    PatrolCoroutine = StartCoroutine(StartPatrolling());
                }

                if (isInDetectionRange(DetectionRange * 1.5f))
                {
                    PatrolCoroutine = null;
                    int RandomVal = Random.Range(0, 3);

                    if (RandomVal == 1)
                        state = States.STRAFE;
                    else
                        state = States.CHASE;
                }
                break;
            case States.CHASE:
                if (NavMeshAgent.enabled)
                {
                    if (NavMesh.SamplePosition(NavMeshAgent.transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                        NavMeshAgent.SetDestination(GetPlayerLocation());
                }

                if (isInDetectionRange(5f))
                {
                    state = States.STRAFE;
                }

                if (!isAttacking)
                    NavMeshAgent.enabled = NavMeshAgent.updateRotation = true;

                break;
            case States.STRAFE:
                if (StrafeCoroutine == null && !isAttacking)
                {
                    if (StrafeElasped > CurrentStrafeT && NavMeshAgent.enabled)
                    {
                        Vector3 dir = (StrafeDirection() * RandomSign()) * Random.Range(2f, 5f);

                        NavMeshPath navMeshPath = new NavMeshPath();
                        if (NavMesh.CalculatePath(rb.position, rb.position + dir, NavMesh.AllAreas, navMeshPath))
                        {
                            StrafeCoroutine = StartCoroutine(StartStrafe(navMeshPath));
                            StrafeElasped = 0;
                        }
                    }
                    StrafeElasped += Time.deltaTime;
                }

                if (HasReachedTargetLocation(GetPlayerLocation()))
                {
                    state = States.BASICATK;
                }

                NavMeshAgent.updateRotation = false;
                NavMeshAgent.enabled = true;
                LookAtPlayerXZ();
                break;
            case States.BASICATK:
                if (!HasReachedTargetLocation(GetPlayerLocation()))
                {
                    state = States.STRAFE;
                }

                if (!isAttacking)
                {
                    if (BasicAttackElapsed > CurrentBasicAttackRate)
                    {
                        if (HasReachedTargetLocation(GetPlayerLocation()))
                        {
                            Animator.SetTrigger("Attack" + Random.Range(1, 1));
                            CurrentBasicAttackRate = Random.Range(DefaultBasicAttackRate, DefaultBasicAttackRate + 0.3f);
                        }
                    }
                    LookAtPlayerXZ();
                    NavMeshAgent.updateRotation = false;
                    NavMeshAgent.enabled = false;
                    BasicAttackElapsed += Time.deltaTime;
                }
                break;
        }
        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
        UpdateShootFireBall();
    }


    public void DealDamageToPlayer()
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position + Vector3.up + transform.forward * 2f, 1f, LayerMask.GetMask("Player", "FF"));
        for (int i = 0; i < Colliders.Length; i++)
        {
            IDamage pc = Colliders[i].GetComponent<IDamage>();
            if (pc != null)
            {
                pc.TakeDamage(pc.GetPointOfContact(), new Elements(Elemental.NONE), GetATK(), this);
                ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().HitEffect, pc.GetPointOfContact(), Quaternion.identity).GetComponent<ParticleSystem>();
                Destroy(hitEffect.gameObject, hitEffect.main.duration);
            }
        }
    }
    private void UpdateShootFireBall()
    {
        if (state != States.STRAFE)
            return;

        int RandomShoot = Random.Range(0, 4);

        switch(RandomShoot)
        {
            case 2:
                if (AttackElapsed > CurrentAttackRate && !isAttacking)
                {
                    Animator.SetTrigger("Fire");
                    LookAtPlayerXZ();
                    AttackElapsed = 0;
                }
                break;
        }

        AttackElapsed += Time.deltaTime;
    }

    protected virtual void ShootFireBall()
    {
        Vector3 dir = (GetPlayerLocation() - FireEmitter.position).normalized;
        Fireball fireBall = Instantiate(FireBallPrefab, FireEmitter.position, Quaternion.identity).GetComponent<Fireball>();
        fireBall.SetElement(elemental);
        Rigidbody r = fireBall.GetComponent<Rigidbody>();
        r.velocity = dir * 15f;
        CurrentAttackRate = Random.Range(DefaultAttackRate, DefaultAttackRate + 0.6f);
    }


    public override bool UpdateDie()
    {
        bool isdead = base.UpdateDie();
        if (isdead)
        {
            DisableAgent();
            if (DieCoroutine == null)
            {
                state = States.DEAD;
                Animator.SetTrigger("Dead");
                DieCoroutine = StartCoroutine(Disappear());
            }
        }
        return isdead;
    }

    private int RandomSign()
    {
        return (Random.value < 0.5f) ? -1 : 1;
    }

    private IEnumerator StartPatrolling()
    {
        yield return new WaitUntil(() => HasReachedTargetLocation(RamdomDestination));
        yield return new WaitForSeconds(Random.Range(1f, 3.5f));
        SetRandomDestination();
        PatrolCoroutine = null;
    }

    private Vector3 StrafeDirection()
    {
        Vector3 dir = GetPlayerLocation() - rb.position;
        dir.y = 0;
        dir.Normalize();
        Vector3 normal = Vector3.Cross(dir, Vector3.up);

        switch (Random.Range(0, 3))
        {
            case 1:
                normal = dir;
                break;
        }

        return normal.normalized;
    }

    private IEnumerator StartStrafe(NavMeshPath navMeshPath)
    {
        NavMeshAgent.SetDestination(navMeshPath.corners[navMeshPath.corners.Length - 1]);

        while (!HasReachedTargetLocation(NavMeshAgent.destination))
        {
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(0.35f, 0.75f));

        switch (Random.Range(0, 4))
        {
            case 2:
                state = States.CHASE;
                break;
        }
        StrafeCoroutine = null;
    }
}
