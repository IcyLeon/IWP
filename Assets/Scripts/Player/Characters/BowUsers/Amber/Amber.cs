using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Amber : BowCharacters, ICoordinateAttack
{
    [SerializeField] Transform CoordinateAttackPivot;
    private float CoodinateTimerElapsed;
    private float CoordinateTimer = 8f;
    private int SkillsArrows = 4;
    private float nextFire;
    [SerializeField] GameObject AmberAuraBurstPrefab;
    private ParticleSystem BurstAura;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CoodinateTimerElapsed = 0;
        PlayerCoordinateAttackManager.OnCoordinateAttack += BasicAtkTrigger;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override bool ElementalBurstTrigger()
    {
        bool canTrigger = base.ElementalBurstTrigger();

        if (canTrigger)
        {
            isBurstActive = true;
            CoodinateTimerElapsed = CoordinateTimer;
            if (BurstAura == null)
                BurstAura = Instantiate(AmberAuraBurstPrefab).GetComponent<ParticleSystem>();

            Destroy(BurstAura.gameObject, CoordinateTimer);
            GetPlayerController().GetPlayerCoordinateAttackManager().Subscribe(this);
        }
        return canTrigger;
    }


    protected override void ElementalSkillTrigger()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerController().CanPerformAction())
            return;

        Vector3 pos = GetPlayerController().GetPlayerOffsetPosition().position;
        for (int i = 0; i < SkillsArrows; i++)
        {
            AmberESkillArrows eSkillArrows = Instantiate(AssetManager.GetInstance().ESkillArrowsPrefab, GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<AmberESkillArrows>();
            eSkillArrows.SetCharacterData(GetCharacterData());
            eSkillArrows.GetRB().velocity = GetShootPositionAndDirection(i);
            eSkillArrows.SetFocalPointContact(GetContactPoint(pos));
        }
        Animator.SetTrigger("Dodge");
        GetCharacterData().ResetElementalSkillCooldown();
    }

    private Vector3 GetShootPositionAndDirection(int i)
    {
        float speed = 35f;
        float angle = 45f;
        float UpOffset = 0.7f;
        Vector3 forward = transform.forward;
        forward.y = 0;

        Vector3 dir = Quaternion.Euler(0, ((-angle / 2) * (SkillsArrows - 1)) + (i * angle), 0) * forward;
        dir.y = 0;
        dir.Normalize();

        Vector3 direction = dir + Vector3.up * UpOffset;
        direction.Normalize();

        return direction * speed;
    }


    private Vector3 GetContactPoint(Vector3 pos)
    {
        float range = 10f;
        Characters NearestEnemy = GetNearestCharacters(range);
        Vector3 forward;
        if (NearestEnemy == null)
        {

            forward = transform.forward;
            forward.y = 0;
            Vector3 endPos = pos + forward * range;
            if (Physics.Raycast(endPos, Vector3.down, out RaycastHit hit))
            {
                endPos = hit.point;
            }
            return endPos;
        }
        else
        {
            forward = NearestEnemy.transform.position - pos;
            forward.Normalize();
            LookAtDirection(forward);
            return NearestEnemy.GetPointOfContact();
        }
    }


    public void UpdateCoordinateAttack()
    {
        CoodinateTimerElapsed -= Time.deltaTime;
        CoodinateTimerElapsed = Mathf.Clamp(CoodinateTimerElapsed, 0f, CoordinateTimer);

        if (BurstAura)
            BurstAura.transform.position = transform.position;
    }

    public bool CoordinateAttackEnded()
    {
        if (GetPlayerController().isDeadState() && IsDead())
            return true;

        return CoodinateTimerElapsed <= 0f;
    }

    public bool CoordinateCanShoot()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + 0.5f;
            return true;
        }
        return false;
    }


    private void BasicAtkTrigger()
    {
        if (!CoordinateCanShoot() || CoordinateAttackEnded())
            return;

        int RandomValue = Random.Range(1, 4);
        for (int j = 0; j < RandomValue; j++)
        {
            CoordinateAttack ca = Instantiate(AssetManager.GetInstance().CoordinateAttackPrefab, CoordinateAttackPivot.position, Quaternion.identity).GetComponent<CoordinateAttack>();
            Vector3 randomDirection = AssetManager.RandomVectorInCone(Vector3.up, 90f);
            ca.transform.position += randomDirection * Random.Range(0.5f, 1f);
            ca.SetCharacterData(GetCharacterData());
        }
    }
}
