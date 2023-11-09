using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Albino : BaseEnemy
{
    public enum States
    {
        PATROL,
        CHASE,
        HEADBUTT,
        SLAM,
        DEAD
    }
    [SerializeField] Collider collider;
    private States state;
    private Coroutine PatrolCoroutine, SlamCoroutine;
    private float LastClickedTime, AttackRate = 0.5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DetectionRange = 5f;
        state = States.PATROL;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateState();

        if (Input.GetKeyDown(KeyCode.Return))
            state = States.SLAM;
    }

    void UpdateState()
    {
        if (NavMeshAgent == null)
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
                    state = States.CHASE;

                break;
            case States.CHASE:
                if (NavMeshAgent.enabled)
                    NavMeshAgent.SetDestination(GetPlayerLocation());

                if (HasReachedTargetLocation(GetPlayerLocation()))
                {
                    state = States.HEADBUTT;
                    NavMeshAgent.enabled = false;
                }

                break;
            case States.HEADBUTT:
                if (Time.time - LastClickedTime > AttackRate)
                {
                    Animator.SetTrigger("Attack" + Random.Range(1, 3));

                    if (!HasReachedTargetLocation(GetPlayerLocation()))
                    {
                        state = States.CHASE;
                        NavMeshAgent.enabled = true;
                    }

                    LastClickedTime = Time.time;
                }
                break;

            case States.SLAM:
                if (SlamCoroutine == null)
                    SlamCoroutine = StartCoroutine(StartSlamAttack());
                break;
        }

        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
        UpdateDie();
    }

    private IEnumerator StartSlamAttack()
    {
        Vector3 SavePosition = GetPlayerLocation();
        float HeightToSlam = 5f;
        float MaxHeightToLaunchAttack = 6f;
        float Diff = SavePosition.y - transform.position.y;

        if (Diff > MaxHeightToLaunchAttack)
        {
            SlamCoroutine = null;
            yield break;
        }
        NavMeshAgent.enabled = false;
        yield return new WaitForSeconds(0.3f);
        collider.isTrigger = true;
        Animator.SetTrigger("Jump");
        

        float elapsed = 0f;
        float duration = 1f;

        Vector3 groundPos = SavePosition;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            groundPos.y = hit.point.y;
        }

        Vector3 targetPosition = groundPos + Vector3.up * HeightToSlam;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }


        while (true)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit GroundHit, 1.0f, ~LayerMask.GetMask("Player")))
            {
                break;
            }
            rb.AddForce(Vector3.down * 100f);
            yield return null;
        }
        collider.isTrigger = false;
        Animator.SetTrigger("Slam");

        yield return new WaitForSeconds(0.3f);
        state = States.CHASE;
        SlamCoroutine = null;
        NavMeshAgent.enabled = true;
    }
    public override void UpdateDie()
    {
        if (GetHealth() <= 0 && DieCoroutine == null)
        {
            state = States.DEAD;
            Animator.SetTrigger("Dead");
            NavMeshAgent.enabled = false;
            DieCoroutine = StartCoroutine(Disappear());
        }
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(8f);
        Destroy(healthBarScript.gameObject);
        Destroy(gameObject);
    }

    private IEnumerator StartPatrolling()
    {
        yield return new WaitUntil(() => HasReachedTargetLocation(RamdomDestination));
        yield return new WaitForSeconds(Random.Range(1f, 3.5f));
        SetRandomDestination();
        PatrolCoroutine = null;
    }

    public void SlamAreaDamage()
    {

    }
}
