using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MageEnemy : BaseEnemy
{
    private MageEnemyStateMachine m_StateMachine;
    [SerializeField] GameObject MageCrystalsOrbPrefab;
    [SerializeField] ParticleSystem FlameEffect;
    private float FireHitInterval = 0.05f, FireHitIntervalElapsed;
    [Header("Fire")]
    [SerializeField] CapsuleCollider FireBreathingCollider;
    [SerializeField] GameObject FireBallPrefab;
    [SerializeField] Transform FireBallPivotTransform;
    [SerializeField] MageEnemyPunch m_PunchCollider;
    private List<MageFireBall> MageFireBallList = new();
    private List<MageOrb> MageOrbList = new();
    private List<FireAreaOfEffect> FireAreaOfEffectList = new();
    private Coroutine MageCrystalCoreCoroutine;
    private GameObject CrystalsParent;
    private bool ChargeAttackEnable;
    private Dictionary<Collider, bool> ChargeColliderList = new();

    private void TurnOnChargeAttackStatus()
    {
        ChargeAttackEnable = true;
    }
    private void TurnOffChargeAttackStatus()
    {
        ChargeAttackEnable = false;
        ChargeColliderList.Clear();
    }

    public void AddToFireAreaOfEffectList(FireAreaOfEffect FireAreaOfEffect)
    {
        FireAreaOfEffectList.Add(FireAreaOfEffect);
    }

    public Transform[] GetAllCrystalsLocation()
    {
        return CrystalsParent.GetComponentsInChildren<Transform>()
            .Where(childTransform => childTransform != CrystalsParent.transform)
            .ToArray();
    }

    private void DestroyAllFireEffect()
    {
        for (int i = 0; i < FireAreaOfEffectList.Count; i++)
        {
            if (FireAreaOfEffectList[i] != null)
            {
                Destroy(FireAreaOfEffectList[i].gameObject);
            }
        }
    }
    public override bool UpdateDie()
    {
        bool isdead = base.UpdateDie();
        if (isdead)
        {
            DestroyAllOrb();
            DestroyAllFireEffect();
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
        FlameEffect.Play();
        FireHitIntervalElapsed = Time.time;
    }
    public void TurnOFFireBreathingCollider()
    {
        GetFireBreathingCollider().enabled = false;
        FlameEffect.Stop();
        FireHitIntervalElapsed = Time.time;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_StateMachine.GetCurrentState() is not MageEnemyChargeAttackState)
            return;

        if (!ChargeAttackEnable)
            return;

        if (!ChargeColliderList.TryGetValue(collision.collider, out bool value))
        {
            PlayerCharacters pc = collision.collider.GetComponent<PlayerCharacters>();
            if (pc != null)
            {
                Vector3 ForceDir = (pc.GetPointOfContact() - GetPointOfContact()).normalized * 15f;
                pc.GetPlayerManager().GetCharacterRB().AddForce(ForceDir, ForceMode.Impulse);
                pc.TakeDamage(pc.GetPointOfContact(), new Elements(Elemental.NONE), GetATK() * 1.8f, this);
            }
            ChargeColliderList.Add(collision.collider, true);
        }
    }

    private void WackPlayer()
    {
        m_PunchCollider.WackPlayer();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (ChargeColliderList.TryGetValue(collision.collider, out bool value))
        {
            ChargeColliderList.Remove(collision.collider);
        }
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
            IDamage PlayerREF = other.GetComponent<IDamage>();
            if (PlayerREF != null)
            {
                if (!PlayerREF.IsDead() && Time.time - FireHitIntervalElapsed > FireHitInterval)
                {
                    PlayerREF.TakeDamage(PlayerREF.GetPointOfContact(), new Elements(Elemental.FIRE), GetATK() * 0.35f, this);
                    FireHitIntervalElapsed = Time.time;
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
            if (CanSpawnReinforcement())
            {
                int randomIndex = Random.Range(0, EM.GetEnemyInfo().GetEnemyContentList().Length);
                BM.SpawnGroundUnitsWithinTerrain(EM.GetEnemyInfo().GetEnemyContentList()[randomIndex].EnemySO);
            }
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
        CrystalsParent = GameObject.FindGameObjectWithTag("MageEntityTargetLocation");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("BossEntity"), LayerMask.NameToLayer("Entity"));
        m_StateMachine = new MageEnemyStateMachine(this);
        base.Start();
    }

    public void SpawnCrystalsOrb()
    {
        int idx = 0;

        if (CrystalsParent == null)
            return;

        int totalLocation = GetAllCrystalsLocation().Length;

        if (totalLocation == 0)
            return;

        for (int i = 0; i < m_StateMachine.MageEnemyData.NoOfCrystalsOrb; i++)
        {
            if (idx >= totalLocation)
                idx -= totalLocation;

            MageOrb MO = Instantiate(MageCrystalsOrbPrefab, GetAllCrystalsLocation()[idx].position, Quaternion.identity).GetComponent<MageOrb>();
            MageOrbList.Add(MO);
            idx++;
        }
        m_StateMachine.MageEnemyData.CrystalsCoreStayElasped = m_StateMachine.MageEnemyData.CrystalsCoreStayDuration;
    }

    // Update is called once per frame
    protected override void Update()
    {
        m_StateMachine.Update();
        base.Update();
        //Debug.Log(m_StateMachine.GetCurrentState());
        UpdateFireBreathing();
        UpdateMageOrbList();
        UpdateMageFireBallList();
        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
        Animator.SetBool("Airborne", IsAirborne());
    }

    public bool CanSpawnReinforcement()
    {
        return EM.HasNotReachSpawnLimit();
    }
    public int GetTotalMageFireBall()
    {
        return MageFireBallList.Count;
    }

    private void SpawnFireBalls()
    { 
        for (int i = 0; i < m_StateMachine.MageEnemyData.NoOfFireball; i++)
        {
            MageFireBall f = Instantiate(FireBallPrefab).GetComponent<MageFireBall>();
            Vector3 dir = Quaternion.Euler(0, 0, Mathf.Rad2Deg * ((Mathf.PI / m_StateMachine.MageEnemyData.NoOfFireball) * (i * 2f))) * Vector3.right;
            f.transform.SetParent(FireBallPivotTransform, true);
            f.transform.localPosition = dir.normalized * 150f;
            f.Init(1f + (i * 0.65f), this);
            MageFireBallList.Add(f);
        }
    }

    private void UpdateMageFireBallList()
    {
        for (int i = FireAreaOfEffectList.Count - 1; i >= 0; i--)
        {
            if (FireAreaOfEffectList[i] == null)
            {
                FireAreaOfEffectList.RemoveAt(i);
            }
        }

        for (int i = MageFireBallList.Count - 1; i >= 0; i--)
        {
            if (MageFireBallList[i] == null)
            {
                MageFireBallList.RemoveAt(i);
            }
        }
    }

    private bool IsAirborne()
    {
        return m_StateMachine.GetCurrentState() is MageEnemyAirborneState;
    }
    public int GetTotalMageNotDestroyed()
    {
        return MageOrbList.Count;
    }
    private void DestroyAllOrb()
    {
        for (int i = MageOrbList.Count - 1; i >= 0; i--)
        {
            if (MageOrbList[i] != null)
            {
                Destroy(MageOrbList[i].gameObject);
            }
        }
    }

    private void UpdateMageOrbList() {

        for (int i = MageOrbList.Count - 1; i >= 0; i--)
        {
            if (MageOrbList[i] == null)
            {
                MageOrbList.RemoveAt(i);
            }
        }

        int total = GetTotalMageNotDestroyed();

        if (total > 0)
        {
            if (m_StateMachine.MageEnemyData.CrystalsCoreStayElasped <= 0)
            {
                if (MageCrystalCoreCoroutine == null)
                {
                    MageCrystalCoreCoroutine = StartCoroutine(MageCrystalCoreMovingTimeout());
                }
            }
            else
            {
                m_StateMachine.MageEnemyData.CrystalsCoreStayElasped -= Time.deltaTime;
            }
        }
    }

    private IEnumerator MageCrystalCoreMovingTimeout()
    {
        AssetManager.GetInstance().OpenMessageNotification("The core's energies are flowing towards its roots...");
        float CrystalsCoreElasped = 0f;
        float CrystalsCoreDuration = 2.5f;

        Vector3[] initialPositions = new Vector3[MageOrbList.Count];
        for (int i = 0; i < MageOrbList.Count; i++)
        {
            if (MageOrbList[i] != null)
            {
                MageOrbList[i].SetTimesUp(true);
                initialPositions[i] = MageOrbList[i].transform.position;
            }
        }

        while (GetTotalMageNotDestroyed() != 0)
        {
            for (int i = 0; i < MageOrbList.Count; i++)
            {
                if (MageOrbList[i] != null)
                {
                    MageOrbList[i].transform.position = Vector3.Lerp(initialPositions[i],
                        CrystalsParent.transform.position,
                        CrystalsCoreElasped / CrystalsCoreDuration);
                }
            }

            CrystalsCoreElasped += Time.deltaTime;
            yield return null;
        }
        NukePlayer(CrystalsParent.transform.position, 100f, 1000f);
        m_StateMachine.MageEnemyData.CrystalsCoreStayElasped = m_StateMachine.MageEnemyData.CrystalsCoreStayDuration;
        MageCrystalCoreCoroutine = null;
    }

    public void NukePlayer(Vector3 TargetPos, float range, float dmg)
    {
        Collider[] AllNearestPlayer = Physics.OverlapSphere(TargetPos, range, LayerMask.GetMask("Player"));
        for (int i = 0; i < AllNearestPlayer.Length; i++)
        {
            IDamage d = AllNearestPlayer[i].GetComponent<IDamage>();
            if (d != null)
            {
                if (!d.IsDead())
                {
                    d.TakeDamage(d.GetPointOfContact(), new Elements(Elemental.FIRE), dmg, this);
                }
            }
        }
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
