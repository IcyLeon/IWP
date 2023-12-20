using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Amber : BowCharacters
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
        PlayerCharacterState = new AmberState(this);
        base.Start();
        CoodinateTimerElapsed = 0;
        PlayerCoordinateAttackManager.OnCoordinateAttack += BasicAtkTrigger;
    }


    protected override bool ElementalBurstTrigger()
    {
        bool canTrigger = base.ElementalBurstTrigger();

        if (canTrigger)
        {
            CoodinateTimerElapsed = CoordinateTimer;
            if (BurstAura == null)
                BurstAura = Instantiate(AmberAuraBurstPrefab).GetComponent<ParticleSystem>();

            Destroy(BurstAura.gameObject, CoordinateTimer);
            GetPlayerManager().GetPlayerController().GetPlayerCoordinateAttackManager().Subscribe(this);
        }
        return canTrigger;
    }

    public void Spawn4Arrows()
    {
        Vector3 pos = GetPlayerManager().transform.position;
        for (int i = 0; i < SkillsArrows; i++)
        {
            AmberESkillArrows eSkillArrows = Instantiate(AssetManager.GetInstance().ESkillArrowsPrefab, GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<AmberESkillArrows>();
            eSkillArrows.SetCharacterData(GetCharacterData());
            eSkillArrows.GetRB().velocity = GetShootPositionAndDirection(i);
            eSkillArrows.SetFocalPointContact(GetContactPoint(pos), NearestTarget);
        }
    }

    private Vector3 GetShootPositionAndDirection(int i)
    {
        float speed = 35f;
        float angle = 45f;
        float UpOffset = 0.5f;
        Vector3 forward = GetPlayerManager().transform.forward;

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
        Vector3 forward;
        if (NearestTarget == null)
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
            forward = NearestTarget.GetPointOfContact() - pos;
            forward.Normalize();
            LookAtDirection(forward);
            return NearestTarget.GetPointOfContact();
        }
    }


    public override void UpdateCoordinateAttack()
    {
        CoodinateTimerElapsed -= Time.deltaTime;
        CoodinateTimerElapsed = Mathf.Clamp(CoodinateTimerElapsed, 0f, CoordinateTimer);

        if (BurstAura)
        {
            if (IsDead())
            {
                Destroy(BurstAura.gameObject);
            }
            else
            {
                BurstAura.transform.position = GetPlayerManager().transform.position;
            }
        }
    }

    public override bool CoordinateAttackEnded()
    {
        if (IsDead())
            return true;

        return CoodinateTimerElapsed <= 0f;
    }

    public override bool CoordinateCanShoot()
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

        if (GetPlayerManager() == null)
            return;

        Vector3 pos = GetPlayerManager().GetPlayerOffsetPosition().position;
        int RandomValue = Random.Range(1, 4);
        for (int j = 0; j < RandomValue; j++)
        {
            CoordinateAttack ca = Instantiate(AssetManager.GetInstance().CoordinateAttackPrefab, CoordinateAttackPivot.position, Quaternion.identity).GetComponent<CoordinateAttack>();
            Vector3 randomDirection = AssetManager.RandomVectorInCone(Vector3.up, 90f);
            ca.transform.position += randomDirection * Random.Range(0.5f, 1f);
            ca.SetCharacterData(GetCharacterData());
            ca.SetTargetPos(GetContactPoint(pos), NearestTarget, GetPlayerManager().transform.forward * 10f);
        }
    }
}
