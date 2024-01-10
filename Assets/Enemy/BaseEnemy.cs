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
    protected float Ratio;
    protected EnemyManager EM;
    private IDamage Target;
    [SerializeField] protected NavMeshAgent NavMeshAgent;
    protected Vector3 RamdomDestination;
    [SerializeField] EnemyValue EnemyValueDropSO;
    private BossEnemyType BossEnemyType;
    protected int Staggering = 1;
    protected int CurrentStaggering;
    private int Level, MaxLevel;
    public Action<BaseEnemy> OnDeadEvent = delegate { };

    protected override void Start()
    {
        EM = EnemyManager.GetInstance();
        CurrentStaggering = 0;
        DetectionRange = 1f;
        BossEnemyType = GetComponent<BossEnemyType>();
        MaxLevel = 50;
        Level = GetRandomLevel();
        if (!BossEnemyType)
        {
            healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab).GetComponent<HealthBarScript>();
            healthBarScript.transform.SetParent(HealthBarPivotParent, true);
            healthBarScript.transform.localPosition = HealthBarPivotParent.localPosition;
            healthBarScript.Init(true, false);
        }

        elementalReaction = new ElementalReaction();
        base.Start();
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.collider.GetComponent<IDamage>() == null)
            return;

        if (!IsDead())
            return;

        Physics.IgnoreCollision(collision.collider, col);
    }

    protected int GetMaxLevel()
    {
        return MaxLevel;
    }
    private int GetRandomLevel()
    {
        int CurrentWave = EM.GetCurrentWave();
        int actualLevel = Random.Range(CurrentWave - 3, CurrentWave + 1);
        actualLevel = Mathf.Clamp(actualLevel, 1, GetMaxLevel());

        return actualLevel;
    }
    
    public float GetDropValue()
    {
        if (EnemyValueDropSO == null)
            return 1;

        return EnemyValueDropSO.BaseDropValue * (GetLevel() - 1) + EnemyValueDropSO.BaseDropValue * (Random.Range(0.5f, 1.5f));
    }
    public override float GetATK()
    {
        return Mathf.RoundToInt(GetCharactersSO().GetAscensionInfo(0).BaseATK + ((GetCharactersSO().GetAscensionInfo(0).BaseMaxATK - GetCharactersSO().GetAscensionInfo(0).BaseATK) / (GetMaxLevel() - 1)) * (GetLevel() - 1));
    }

    public override float GetMaxHealth()
    {
        return Mathf.RoundToInt(GetCharactersSO().GetAscensionInfo(0).BaseHP + ((GetCharactersSO().GetAscensionInfo(0).BaseMaxHP - GetCharactersSO().GetAscensionInfo(0).BaseHP) / (GetMaxLevel() - 1)) * (GetLevel() - 1));
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

                IInteract interact = goList[i].GetComponent<IInteract>();
                if (interact == null)
                {
                    goCopy.Add(damage);
                }
                else
                {
                    if (!interact.CanInteract())
                        goCopy.Add(damage);
                }
            }
        }
        return goCopy.ToArray();
    }

    protected override void HitEvent(Elements e, IDamage d)
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

    public override int GetLevel()
    {
        return Level;
    }

    protected virtual void UpdateState()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        Target = GetNearestPlayerObject();
        base.Update();
        UpdateHealthBar();

        if (GetElementalReaction() != null)
            GetElementalReaction().UpdateElementsList();

        UpdateOutofBound();
    }

    public override Vector3 GetPointOfContact()
    {
        Vector3 temp = transform.position;
        if (GetCollider() != null)
        {
            temp = GetCollider().bounds.center;
        }
        return temp;
    }

    public Collider GetCollider()
    {
        return col;
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

    public Vector3 GetTargetDirection()
    {
        if (GetPlayerLocation() == default(Vector3))
            return default(Vector3);

        return GetPlayerLocation() - transform.position;
    }

    public void LookAtPlayerXZ()
    {
        Vector3 forward = GetTargetDirection();
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        GetRB().rotation = Quaternion.Euler(0, angle, 0);
    }

    public Rigidbody GetRB()
    {
        return rb;
    }


    protected IEnumerator Disappear(float delay = 8f)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public override bool UpdateDie()
    {
        bool isdead = base.UpdateDie();
        if (isdead)
        {
            if (healthBarScript)
            {
                Destroy(healthBarScript.gameObject);

                EM.CallOnEnemyKilled(this);
                OnDeadEvent?.Invoke(this);
            }
        }
        return isdead;
    }

    private void UpdateHealthBar()
    {
        if (healthBarScript)
        {
            healthBarScript.SliderInvsibleOnlyFullHealth();
            healthBarScript.UpdateLevel(GetLevel());
            healthBarScript.UpdateShield(GetCurrentElementalShield(), 0, GetElementalShield());
        }
    }

    public override Elements TakeDamage(Vector3 pos, Elements elements, float amt, IDamage source, bool callHitInfo = true)
    {
        float actualamt = amt;
        if (GetCurrentElementalShield() > 0) // have to change later since it is hardcoded
        {
            actualamt *= 0.15f;
            SetCurrentElementalShield(GetCurrentElementalShield() - amt);
        }

        Elements e = base.TakeDamage(pos, elements, actualamt, source, callHitInfo);

        if (healthBarScript && !BossEnemyType)
        {
            if (healthBarScript.GetElementsIndicator() == null)
                healthBarScript.SetElementsIndicator(this);
        }

        if (e.GetElements() != Elemental.NONE)
        {
            PlayerManager PM = (PlayerManager)source.GetSource();
            if (PM != null)
            {
                ElementalOrb go = Instantiate(AssetManager.GetInstance().ElementalOrbPrefab, GetPointOfContact(), Quaternion.identity).GetComponent<ElementalOrb>();
                go.SetSource(PM);
            }
        }

        return e;
    }

    public bool HasReachedTargetLocation(Vector3 target)
    {
        if (GetNavMeshAgent() == null)
            return false;

        return (transform.position - target).magnitude <= GetNavMeshAgent().stoppingDistance + 0.25f; // slide offset to prevent jerking
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
