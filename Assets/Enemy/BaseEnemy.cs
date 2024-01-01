using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BaseEnemy : Characters
{
    [SerializeField] protected Collider col;
    [SerializeField] protected Rigidbody rb;
    protected float DetectionRange;
    private ElementsIndicator elementsIndicator;
    protected float Ratio;
    private IDamage Target;
    [SerializeField] protected NavMeshAgent NavMeshAgent;
    private EnemyManager EM;
    protected Vector3 RamdomDestination;
    [SerializeField] EnemyValue EnemyValueDropSO;
    private BossEnemyType BossEnemyType;
    protected int Staggering = 1;
    protected int CurrentStaggering;

    protected override void Start()
    {
        EM = EnemyManager.GetInstance();
        base.Start();
        CurrentStaggering = 0;
        CurrentHealth = GetMaxHealth();
        DetectionRange = 1f;
        Level = 1;

        BossEnemyType = GetComponent<BossEnemyType>();

        if (!BossEnemyType)
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

    protected override void OnHit(Elements e, IDamage d)
    {
        PlayerManager.CallEntityHitSendInfo(e, d);
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

    public void LookAtPlayerXZ()
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
            healthBarScript.UpdateShield(GetCurrentElementalShield(), 0, GetElementalShield());

            if (elementsIndicator)
            {
                elementsIndicator.transform.position = HealthBarPivotParent.position + Vector3.up * 1.8f;
            }
        }
    }

    public override Elements TakeDamage(Vector3 pos, Elements elements, float amt, bool callHitInfo = true)
    {
        float actualamt = amt;
        if (GetCurrentElementalShield() > 0) // have to change later since it is hardcoded
        {
            actualamt *= 0.3f;
            SetCurrentElementalShield(GetCurrentElementalShield() - amt);
        }

        Elements e = base.TakeDamage(pos, elements, actualamt, callHitInfo);

        if (elementsIndicator == null && !BossEnemyType)
        {
            elementsIndicator = Instantiate(AssetManager.GetInstance().ElementalContainerPrefab, transform).GetComponent<ElementsIndicator>();
            elementsIndicator.SetCharacters(this);
        }

        if (e.GetElements() != Elemental.NONE)
        {
            ElementalOrb go = Instantiate(AssetManager.GetInstance().ElementalOrbPrefab, GetPointOfContact(), Quaternion.identity).GetComponent<ElementalOrb>();
            go.SetSource(CharacterManager.GetInstance().GetPlayerManager());
        }

        return e;
    }

    protected virtual void ElementReactionHit(ElementalReactionsTrigger e)
    {

    }

    public bool HasReachedTargetLocation(Vector3 target)
    {
        if (GetNavMeshAgent() == null)
            return false;

        return (transform.position - target).magnitude <= GetNavMeshAgent().stoppingDistance + 0.25f;
    }

    protected void SetRandomDestination()
    {
        if (GetNavMeshAgent() == null)
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


    public Vector3 GetPlayerLocation()
    {
        if (Target == null)
            return default(Vector3);

        return Target.GetPointOfContact();
    }

    public bool isInDetectionRange(float range)
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
    public NavMeshAgent GetNavMeshAgent()
    {
        return NavMeshAgent;
    }

    public void DisableAgent()
    {
        if (GetNavMeshAgent() == null)
            return;

        GetNavMeshAgent().updatePosition = false;
        GetNavMeshAgent().updateRotation = false;
        GetNavMeshAgent().velocity = Vector3.zero;
        if (GetNavMeshAgent().enabled)
        {
            GetNavMeshAgent().isStopped = true;
            GetNavMeshAgent().enabled = false;
        }
    }

    public void EnableAgent()
    {
        if (GetNavMeshAgent() == null)
            return;

        GetNavMeshAgent().enabled = true;
        GetNavMeshAgent().updatePosition = true;
        GetNavMeshAgent().updateRotation = true;
        GetNavMeshAgent().isStopped = false;
    }
}
