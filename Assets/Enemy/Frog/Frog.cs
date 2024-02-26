using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Frog : BaseEnemy
{
    public enum States
    {
        PATROL,
        CHASE,
        STRAFE,
        BASICATK,
    }
    private States state;
    private Coroutine PatrolCoroutine, HopCoroutine;
    private float hopHeight = 0.5f;
    private float hopFrequency = 0.2f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DetectionRange = 8f;
        state = States.PATROL;

        StartCoroutine(HopPeriodically());

    }

    protected override void Update()
    {
        base.Update();
        UpdateState();
    }

    // Update is called once per frame
    protected override void UpdateState()
    {
        if (NavMeshAgent == null)
            return;

        switch(state)
        {
            case States.PATROL:
                if (PatrolCoroutine == null)
                {
                    SetRandomDestination();
                    DisableAgent();
                    PatrolCoroutine = StartCoroutine(StartPatrolling());
                }

                if (isInDetectionRange(DetectionRange * 1.5f))
                {
                    state = States.CHASE; 
                }
                break;
            case States.CHASE:
                if (NavMeshAgent.enabled)
                    NavMeshAgent.SetDestination(GetPlayerLocation());
                

                break;
            case States.STRAFE:

                break;
            case States.BASICATK:

                break;
        }
    }

    private IEnumerator StartPatrolling()
    {
        yield return new WaitUntil(() => HasReachedTargetLocation(RamdomDestination));
        yield return new WaitForSeconds(Random.Range(1f, 3.5f));
        SetRandomDestination();
        PatrolCoroutine = null;
    }


    IEnumerator HopPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(hopFrequency);
            if (HopCoroutine == null && !HasReachedTargetLocation(RamdomDestination))
            {
                HopCoroutine = StartCoroutine(Hop());
            }
        }
    }

    IEnumerator Hop()
    {
        EnableAgent();

        if (state == States.PATROL)
            NavMeshAgent.SetDestination(RamdomDestination);

        Vector3 initialPosition = Model.transform.localPosition;
        Vector3 targetPosition = Model.transform.localPosition + Vector3.up * hopHeight;

        float duration = hopFrequency / 2;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            Model.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Model.transform.localPosition = targetPosition;

        elapsed = 0f;
        while (elapsed < duration)
        {
            Model.transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Model.transform.localPosition = initialPosition;

        DisableAgent();
        yield return new WaitForSeconds(0.5f);
        HopCoroutine = null;
    }

}
