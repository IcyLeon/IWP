using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CurrentCharacterArtifacts;
using UnityEngine.UIElements;

public class MageEnemy : BaseEnemy
{
    private MageEnemyStateMachine m_StateMachine;
    [SerializeField] GameObject MageCrystalsOrbPrefab;
    private float FireHitInterval = 0.15f, FireHitIntervalElapsed;
    [Header("Fire")]
    [SerializeField] CapsuleCollider FireBreathingCollider;
    [SerializeField] GameObject FireBallPrefab;
    [SerializeField] Transform FireBallPivotTransform;

    private List<MageOrb> MageOrbList;
    private Coroutine MageCrystalCoreCoroutine;
    private GameObject CrystalsParent;

    public Transform[] GetAllCrystalsLocation()
    {
        return CrystalsParent.GetComponentsInChildren<Transform>()
            .Where(childTransform => childTransform != CrystalsParent.transform)
            .ToArray();
    }

    public override bool UpdateDie()
    {
        bool isdead = base.UpdateDie();
        if (isdead)
        {
            DestroyAllOrb();
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
        CrystalsParent = GameObject.FindGameObjectWithTag("MageEntityTargetLocation");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("BossEntity"), LayerMask.NameToLayer("Entity"));
        m_StateMachine = new MageEnemyStateMachine(this);
        MageOrbList = new();
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
        Animator.SetFloat("Velocity", NavMeshAgent.velocity.magnitude, 0.15f, Time.deltaTime);
        Animator.SetBool("Airborne", IsAirborne());
    }

    private void SpawnFireBalls()
    { 
        for (int i = 0; i < m_StateMachine.MageEnemyData.NoOfFireball; i++)
        {
            MageFireBall f = Instantiate(FireBallPrefab, FireBallPivotTransform).GetComponent<MageFireBall>();
            Vector2 dir = Quaternion.Euler(0, 0, Mathf.Rad2Deg * ((Mathf.PI / m_StateMachine.MageEnemyData.NoOfFireball) * (i * 2f))) * FireBallPivotTransform.transform.right;
            f.transform.localPosition = dir.normalized * 100f;
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
        NukePlayer(CrystalsParent.transform.position);
        m_StateMachine.MageEnemyData.CrystalsCoreStayElasped = m_StateMachine.MageEnemyData.CrystalsCoreStayDuration;
        MageCrystalCoreCoroutine = null;
    }

    private void NukePlayer(Vector3 TargetPos)
    {
        IDamage[] AllNearestPlayer = GetAllNearestCharacters(TargetPos, 100f, LayerMask.GetMask("Player"));
        for (int i = 0; i < AllNearestPlayer.Length; i++)
        {
            IDamage d = AllNearestPlayer[i];
            if (d != null)
            {
                if (!d.IsDead())
                {
                    d.TakeDamage(d.GetPointOfContact(), new Elements(Elemental.FIRE), 1000f);
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
