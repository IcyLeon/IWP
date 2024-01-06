using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Amber : BowCharacters, ICoordinateAttack
{
    [SerializeField] Transform CoordinateAttackPivot;
    private float CoodinateTimerElapsed;
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

    public void SpawnAura()
    {
        CoodinateTimerElapsed = GetCharacterData().GetPlayerCharacterSO().ElementalBurstTimer;
        if (BurstAura == null)
            BurstAura = Instantiate(AmberAuraBurstPrefab).GetComponent<ParticleSystem>();
        Destroy(BurstAura.gameObject, CoodinateTimerElapsed);
    }

    public void Spawn4Arrows()
    {
        Vector3 targetPos = GetContactPoint();
        Vector3 lookatDir = targetPos - GetPlayerManager().transform.position;
        lookatDir.y = 0;
        lookatDir.Normalize();
        LookAtDirection(lookatDir);

        for (int i = 0; i < SkillsArrows; i++)
        {
            AmberESkillArrows eSkillArrows = Instantiate(AssetManager.GetInstance().ESkillArrowsPrefab, GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<AmberESkillArrows>();
            eSkillArrows.SetCharacterData(this);
            eSkillArrows.GetRB().velocity = GetShootPositionAndDirection(i);
            eSkillArrows.SetFocalPointContact(targetPos, NearestTarget);
        }
    }

    private Vector3 GetShootPositionAndDirection(int i)
    {
        float speed = 35f;
        float angle = 45f;
        float UpOffset = 0.5f;
        Vector3 forward = GetPlayerManager().transform.forward;
        if (NearestTarget != null)
        {
            forward = NearestTarget.GetPointOfContact() - GetPlayerManager().GetPlayerOffsetPosition().position;
            forward.Normalize();
        }

        Vector3 dir = Quaternion.Euler(0, ((-angle / 2) * (SkillsArrows - 1)) + (i * angle), 0) * forward;
        dir.y = 0;
        dir.Normalize();

        Vector3 direction = dir + Vector3.up * UpOffset;
        direction.Normalize();

        return direction * speed;
    }


    private Vector3 GetContactPoint()
    {
        float range = 10f;
        if (NearestTarget == null)
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            Vector3 endPos = GetPlayerManager().GetPlayerOffsetPosition().position + forward * range;
            if (Physics.Raycast(endPos, Vector3.down, out RaycastHit hit))
            {
                endPos = hit.point;
            }
            return endPos;
        }

        return NearestTarget.GetPointOfContact();
    }


    public void UpdateCoordinateAttack()
    {
        CoodinateTimerElapsed -= Time.deltaTime;
        CoodinateTimerElapsed = Mathf.Clamp(CoodinateTimerElapsed, 0f, GetCharacterData().GetPlayerCharacterSO().ElementalBurstTimer);

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

    public bool CoordinateAttackEnded()
    {
        if (IsDead())
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

        if (GetPlayerManager() == null)
            return;

        Vector3 targetPos = GetContactPoint();

        int RandomValue = Random.Range(1, 4);
        for (int j = 0; j < RandomValue; j++)
        {
            CoordinateAttack ca = Instantiate(AssetManager.GetInstance().CoordinateAttackPrefab, CoordinateAttackPivot.position, Quaternion.identity).GetComponent<CoordinateAttack>();
            Vector3 randomDirection = AssetManager.RandomVectorInCone(Vector3.up, 90f);
            ca.transform.position += randomDirection * Random.Range(0.5f, 1f);
            ca.SetCharacterData(this);
            ca.SetTargetPos(targetPos, NearestTarget, GetPlayerManager().transform.forward * 10f);
        }
    }
}
