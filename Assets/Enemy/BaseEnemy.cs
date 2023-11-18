using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : Characters
{
    [SerializeField] protected Collider collider;
    protected float DetectionRange;
    private ElementsIndicator elementsIndicator;
    protected float Ratio;
    private PlayerController Player;
    [SerializeField] protected NavMeshAgent NavMeshAgent;
    private EnemyManager EM;
    protected Vector3 RamdomDestination;

    protected int Staggering = 1;
    protected int CurrentStaggering;

    // Start is called before the first frame update
    protected override void Start()
    {
        EM = EnemyManager.GetInstance();
        base.Start();
        CurrentStaggering = 0;
        CurrentHealth = GetMaxHealth();
        DetectionRange = 1f;
        Level = 1;
        healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab, transform).GetComponent<HealthBarScript>();
        elementalReaction = new ElementalReaction();
        OnElementReactionHit += ElementReactionHit;
        Player = CharacterManager.GetInstance().GetPlayerController();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateHealthBar();

        if (elementsIndicator)
            elementsIndicator.transform.position = GetModel().transform.position + Vector3.up * 2.1f;

        if (GetElementalReaction() != null)
            GetElementalReaction().UpdateElementsList();
    }

    protected void TriggerStaggering(int amt = 1, bool conditionToTrigger = true)
    {
        if (!isAttacking)
        {
            CurrentStaggering += amt;
            if (CurrentStaggering >= Staggering && conditionToTrigger)
            {
                Animator.SetTrigger("Hit");
                CurrentStaggering = 0;
            }
        }
    }

    protected void LookAtPlayerXZ()
    {
        Vector3 forward = GetPlayerLocation() - transform.position;
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }


    protected IEnumerator Disappear()
    {
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }

    public override bool UpdateDie()
    {
        bool isdead = base.UpdateDie();
        if (isdead)
        {
            if (healthBarScript)
                Destroy(healthBarScript.gameObject);
            if (elementsIndicator)
                Destroy(elementsIndicator.gameObject);

            EM.SetCurrentEnemyDefeated(EM.GetCurrentEnemyDefeated() + 1);

        }
        return isdead;
    }

    private void UpdateHealthBar()
    {
        if (healthBarScript)
        {
            healthBarScript.transform.position = GetModel().transform.position + Vector3.up * 1.5f;
            healthBarScript.SliderInvsibleOnlyFullHealth();
        }
    }

    public override Elements TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        Elements e = base.TakeDamage(pos, elements, amt);

        if (elementsIndicator == null)
        {
            elementsIndicator = Instantiate(AssetManager.GetInstance().ElementalContainerPrefab).GetComponent<ElementsIndicator>();
            elementsIndicator.SetCharacters(this);
        }

        if (e.GetElements() != Elemental.NONE)
        {
            GameObject go = Instantiate(AssetManager.GetInstance().ElementalOrbPrefab, transform.position, Quaternion.identity);
        }

        return e;
    }

    protected virtual void ElementReactionHit(ElementalReactionsTrigger e)
    {

    }

    protected bool HasReachedTargetLocation(Vector3 target)
    {
        if (NavMeshAgent == null)
            return false;

        return (transform.position - target).magnitude <= NavMeshAgent.stoppingDistance + 0.25f;
    }

    protected void SetRandomDestination()
    {
        if (NavMeshAgent == null)
            return;

        Vector3 randomPoint = transform.position + new Vector3(Random.Range(-DetectionRange, DetectionRange), 0, Random.Range(-DetectionRange, DetectionRange));

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, DetectionRange, NavMesh.AllAreas))
        {
            RamdomDestination = hit.position;
        }
        else
        {
            SetRandomDestination();
        }
    }


    protected Vector3 GetPlayerLocation()
    {
        return Player.GetPlayerOffsetPosition().position;
    }

    protected bool isInDetectionRange(float range)
    {
        return (transform.position - GetPlayerLocation()).magnitude <= range;
    }
    public override float GetMaxHealth()
    {
        return BaseMaxHealth * (1 + Ratio);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnElementReactionHit -= ElementReactionHit;
    }


    protected void DisableAgent()
    {
        NavMeshAgent.updatePosition = false;
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.velocity = Vector3.zero;
        if (NavMeshAgent.enabled)
        {
            NavMeshAgent.isStopped = true;
            NavMeshAgent.enabled = false;
        }
    }

    protected void EnableAgent()
    {
        NavMeshAgent.enabled = true;
        NavMeshAgent.updatePosition = true;
        NavMeshAgent.updateRotation = true;
        NavMeshAgent.isStopped = false;
    }
}
