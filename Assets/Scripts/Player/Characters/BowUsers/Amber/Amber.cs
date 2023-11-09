using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amber : BowCharacters, ICoordinateAttack
{
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
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void FixedUpdate()
    {
        if (!GetBurstCamera().gameObject.activeSelf)
            GetPlayerController().UpdatePhysicsMovement();

        GetPlayerController().UpdateTargetRotation();
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
        if (!GetCharacterData().CanTriggerESKill() || GetPlayerController().GetPlayerActionStatus() != PlayerActionStatus.IDLE)
            return;

        for (int i = 0; i < SkillsArrows; i++)
        {
            AmberESkillArrows eSkillArrows = Instantiate(AssetManager.GetInstance().ESkillArrowsPrefab, GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<AmberESkillArrows>();
            eSkillArrows.SetCharacterData(GetCharacterData());
            eSkillArrows.GetRB().velocity = GetShootDirection(transform.position) * 100f + Vector3.up * 65f;
            eSkillArrows.SetFocalPointContact(GetContactPoint(transform.position));
        }
        Animator.SetTrigger("Dodge");
        GetCharacterData().ResetElementalSkillCooldown();
    }

    private Vector3 GetShootDirection(Vector3 pos)
    {
        Vector3 dir = AssetManager.RandomVectorInCone(GetDirection(pos), 75f);
        dir.y = Mathf.Abs(dir.y);
        return dir;
    }

    private Vector3 GetDirection(Vector3 pos)
    {
        return (GetContactPoint(pos) - GetEmitterPivot().position).normalized;
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
            Vector3 endPos = transform.position + forward * range;
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
            return NearestEnemy.transform.position;
        }
    }

    public float GetCoordinateAttackTimer()
    {
        return CoordinateTimer;
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
}
