using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Albino : BaseEnemy
{
    public enum States
    {
        PATROL,
        CHASE,
        HEADBUTT,
        SLAM,
        PREPARING,
        DEAD
    }
    private States state;
    private Coroutine PatrolCoroutine, SlamCoroutine;
    private bool JumpOnAir;
    private float DefaultAttackRate = 0.4f, CurrentAttackRate;
    private float AttackElapsed;
    private float StrafeElasped;
    private float StrafeT = 0.2f;
    private Coroutine StrafeCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CurrentAttackRate = DefaultAttackRate;
        JumpOnAir = false;
        DetectionRange = 10f;
        StrafeElasped = AttackElapsed = 0f;
        Staggering = 15;
        state = States.CHASE;
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

        if (state == States.PATROL)
            state = States.CHASE;

        base.HitEvent(ER, e, dmg);

        if (e != null)
            currentStag += 2;
        if (ER != null)
            currentStag += 5;

        TriggerStaggering(currentStag, state != States.SLAM);
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
                    state = States.CHASE;
                }

                break;
            case States.CHASE:
                if (NavMeshAgent.enabled)
                {
                    if (NavMesh.SamplePosition(NavMeshAgent.transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                    {
                        NavMeshAgent.SetDestination(GetPlayerLocation());
                    }
                }

                if (HasReachedTargetLocation(GetPlayerLocation()))
                {
                    state = States.HEADBUTT;
                }

                if (!isAttacking)
                    NavMeshAgent.enabled = NavMeshAgent.updateRotation = true;

                break;

            case States.PREPARING:
                if (StrafeCoroutine == null && !isAttacking) {
                    if (HasReachedTargetLocation(GetPlayerLocation()))
                    {
                        state = States.HEADBUTT;
                        StrafeElasped = 0;
                    }

                    if (StrafeElasped > StrafeT && NavMeshAgent.enabled)
                    {
                        Vector3 dir = (StrafeDirection() * RandomSign()) * Random.Range(2f, 5f);

                        NavMeshPath navMeshPath = new NavMeshPath();
                        if (NavMesh.CalculatePath(rb.position, rb.position + dir, NavMesh.AllAreas, navMeshPath))
                        {
                            StrafeCoroutine = StartCoroutine(StartStrafe(navMeshPath));
                        }
                    }
                    StrafeElasped += Time.deltaTime;
                }

                LookAtPlayerXZ();
                NavMeshAgent.updateRotation = false;
                NavMeshAgent.enabled = true;

                break;
            case States.HEADBUTT:
                if (!isAttacking && state != States.SLAM)
                {
                    if (!HasReachedTargetLocation(GetPlayerLocation()) && NavMeshAgent.enabled)
                    {
                        state = States.CHASE;
                    }


                    if (AttackElapsed > CurrentAttackRate)
                    {
                        int attackRandom = Random.Range(0, 4);
                        if (attackRandom == 2)
                        {
                            state = States.SLAM;
                            CurrentAttackRate = Random.Range(DefaultAttackRate, DefaultAttackRate + 0.3f);
                        }

                        if (HasReachedTargetLocation(GetPlayerLocation()) && state != States.SLAM)
                        {
                            Animator.SetTrigger("Attack" + Random.Range(1, 3));
                            CurrentAttackRate = Random.Range(DefaultAttackRate, DefaultAttackRate + 0.3f);
                        }
                        AttackElapsed = 0;
                    }
                    LookAtPlayerXZ();
                    NavMeshAgent.updateRotation = false;
                    NavMeshAgent.enabled = false;
                    AttackElapsed += Time.deltaTime;
                }
                else
                {
                    AttackElapsed = 0;
                }

                break;

            case States.SLAM:
                if (StrafeCoroutine != null)
                {
                    StopCoroutine(StrafeCoroutine);
                    StrafeCoroutine = null;
                }
                if (SlamCoroutine == null && !isAttacking)
                {
                    NavMeshAgent.updateRotation = false;
                    NavMeshAgent.enabled = false;
                    SlamCoroutine = StartCoroutine(StartSlamAttack());
                }
                break;
        }
        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
    }
    private int RandomSign()
    {
        return (Random.value < 0.5f) ? -1 : 1;
    }

    public void ChangeState(States states)
    {
        state = states;
    }

    private IEnumerator StartStrafe(NavMeshPath navMeshPath)
    {
        NavMeshAgent.SetDestination(navMeshPath.corners[navMeshPath.corners.Length - 1]);

        while (!HasReachedTargetLocation(NavMeshAgent.destination))
        {
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(0.45f, 1.2f));

        switch (Random.Range(0, 4))
        {
            case 1:
                state = States.SLAM;
                break;
            case 2:
                state = States.CHASE;
                break;
        }
        StrafeElasped = 0;
        StrafeCoroutine = null;
    }

    private Vector3 StrafeDirection()
    {
        Vector3 dir = GetPlayerLocation() - rb.position;
        dir.y = 0;
        dir.Normalize();
        Vector3 normal = Vector3.Cross(dir, Vector3.up);

        switch(Random.Range(0, 3))
        {
            case 1:
                normal = dir;
                break;
        }

        return normal.normalized;
    }

    private IEnumerator StartSlamAttack()
    {
        Vector3 SavePosition = GetPlayerLocation();
        float HeightToSlam = 4f;
        float MaxHeightToLaunchAttack = 8f;
        float Diff = SavePosition.y - transform.position.y;

        if (Mathf.Abs(Diff) > MaxHeightToLaunchAttack)
        {
            state = States.CHASE;
            col.isTrigger = false;
            Animator.SetTrigger("Cancel");
            SlamCoroutine = null;
        }
        Animator.SetTrigger("Jump");
        SetisAttacking(true);
        yield return new WaitUntil(() => JumpOnAir);
        col.isTrigger = true;

        SavePosition = GetPlayerLocation();
        Vector3 groundPos = SavePosition;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            groundPos.y = hit.point.y;
        }

        float elapsed = 0f;
        float duration = 0.6f;

        Vector3 targetPosition = groundPos + Vector3.up * HeightToSlam;
        while (elapsed < duration)
        {
            rb.position = Vector3.Lerp(rb.position, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit GroundHit, 0.5f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore))
            {
                SlamAreaDamage();
                break;
            }
            rb.AddForce(Vector3.down * 1000f * Time.deltaTime);
            yield return null;
        }
        col.isTrigger = false;
        Animator.SetTrigger("Slam");
        JumpOnAir = false;
        yield return new WaitForSeconds(0.25f);
        ResetAttack();
        state = States.CHASE;
        SlamCoroutine = null;
    }
    public void CanJumpOnAir()
    {
        JumpOnAir = true;
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

    private IEnumerator StartPatrolling()
    {
        yield return new WaitUntil(() => HasReachedTargetLocation(RamdomDestination));
        yield return new WaitForSeconds(Random.Range(1f, 3.5f));
        SetRandomDestination();
        PatrolCoroutine = null;
    }
    public void DealDamageToPlayer()
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position + Vector3.up + transform.forward * 2f, 1f, LayerMask.GetMask("Player"));
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
    public void SlamAreaDamage()
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, 2.5f, LayerMask.GetMask("Player"));
        for (int i = 0; i < Colliders.Length; i++)
        {
            PlayerCharacters pc = Colliders[i].GetComponent<PlayerCharacters>();
            if (pc != null)
            {
                if (pc.GetBurstActive())
                    return;

                Elements e = pc.TakeDamage(pc.transform.position, new Elements(Elemental.NONE), 100f, this);
                if (e != null)
                    pc.GetPlayerManager().GetCharacterRB().AddForce(((pc.GetPlayerManager().GetCharacterRB().position - pc.transform.position).normalized + Vector3.up).normalized * 15f, ForceMode.Impulse); ;
            }
        }
        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().HitEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(hitEffect.gameObject, hitEffect.main.duration);
    }
}