using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : Characters
{
    [SerializeField] protected Collider col;
    protected float DetectionRange;
    private ElementsIndicator elementsIndicator;
    protected float Ratio;
    private IDamage Target;
    [SerializeField] protected NavMeshAgent NavMeshAgent;
    private EnemyManager EM;
    protected Vector3 RamdomDestination;
    [SerializeField] EnemyValue EnemyValueDropSO;

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
    }

    public float GetDropValue()
    {
        if (EnemyValueDropSO == null)
            return 1;

        return EnemyValueDropSO.BaseDropValue * (GetLevel() - 1) + EnemyValueDropSO.BaseDropValue * (Random.Range(0.5f, 1.5f));
    }

    private IDamage[] GetAllFriendlyAllies()
    {
        GameObject[] goList = GameObject.FindGameObjectsWithTag("Player");
        List<IDamage> goCopy = new();
        for (int i = 0; i < goList.Length; i++)
        {
            IDamage damage = goList[i].GetComponent<IDamage>();
            if (damage != null)
            {
                goCopy.Add(damage);
            }
        }
        return goCopy.ToArray();
    }

    private IDamage GetNearestPlayerObject()
    {
        IDamage[] goList = GetAllFriendlyAllies();

        if (goList.Length == 0)
            return null;

        IDamage nearest = goList[0];
        for (int i = 0; i < goList.Length; i++)
        {
            float dist1 = Vector3.Distance(goList[i].GetPointOfContact(), GetPointOfContact());
            float dist2 = Vector3.Distance(nearest.GetPointOfContact(), GetPointOfContact());

            if (dist1 < dist2)
            {
                nearest = goList[i];
            }
        }
        return nearest;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Target == null)
        {
            Target = GetNearestPlayerObject();
        }
        base.Update();
        UpdateHealthBar();

        if (GetElementalReaction() != null)
            GetElementalReaction().UpdateElementsList();

        UpdateOutofBound();
    }

    public override Vector3 GetPointOfContact()
    {
        return col.bounds.center;
    }

    private void UpdateOutofBound()
    {
        if (transform.position.y <= -100f)
        {
            SetHealth(0);
            UpdateDie();
            Destroy(gameObject);
        }
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

            EM.CallOnEnemyKilled(this);
        }
        return isdead;
    }

    private void UpdateHealthBar()
    {
        if (healthBarScript)
        {
            healthBarScript.transform.position = HealthBarPivotParent.position + Vector3.up;
            healthBarScript.SliderInvsibleOnlyFullHealth();
        }
        if (elementsIndicator)
        {
            elementsIndicator.transform.position = HealthBarPivotParent.position + Vector3.up * 2f;
        }
    }

    public override Elements TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        Elements e = base.TakeDamage(pos, elements, amt);

        if (elementsIndicator == null)
        {
            elementsIndicator = Instantiate(AssetManager.GetInstance().ElementalContainerPrefab, transform).GetComponent<ElementsIndicator>();
            elementsIndicator.SetCharacters(this);
        }

        if (e.GetElements() != Elemental.NONE)
        {
            ElementalOrb go = Instantiate(AssetManager.GetInstance().ElementalOrbPrefab, transform.position, Quaternion.identity).GetComponent<ElementalOrb>();
            go.SetSource(CharacterManager.GetInstance().GetPlayerManager());
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
        if (Target == null)
            return default(Vector3);

        return Target.GetPointOfContact();
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
        if (NavMeshAgent == null)
            return;

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
        if (NavMeshAgent == null)
            return;

        NavMeshAgent.enabled = true;
        NavMeshAgent.updatePosition = true;
        NavMeshAgent.updateRotation = true;
        NavMeshAgent.isStopped = false;
    }
}
