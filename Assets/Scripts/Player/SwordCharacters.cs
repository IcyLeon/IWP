using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordCharacters : PlayerCharacters
{
    [SerializeField] protected GameObject SwordModel;
    [SerializeField] protected Transform EmitterPivot;
    protected int BasicAttackPhase = 0;
    private int MaxAttackPhase;
    protected Elemental CurrentElement;
    private float LastClickedTime, AttackRate = 0.08f;

    public GameObject GetSwordModel()
    {
        return SwordModel;
    }
    public virtual void SpawnSlash()
    {
        BasicAttackTrigger();
        AssetManager.GetInstance().SpawnSlashEffect(GetEmitterPivot());
        Collider[] Colliders = Physics.OverlapSphere(GetPlayerManager().GetPlayerOffsetPosition().position + Vector3.up + transform.forward * 2f, 2f, LayerMask.GetMask("Entity"));
        foreach (Collider other in Colliders)
        {
            IDamage damageObj = other.GetComponent<IDamage>();
            if (damageObj != null)
            {
                if (!damageObj.IsDead())
                {
                    Vector3 hitPosition = other.transform.position;

                    if (other is MeshCollider meshCollider)
                    {
                        hitPosition = meshCollider.ClosestPointOnBounds(other.transform.position);
                    }

                    ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, hitPosition, Quaternion.identity).GetComponent<ParticleSystem>();
                    Destroy(hitEffect.gameObject, hitEffect.main.duration);
                    damageObj.TakeDamage(damageObj.GetPointOfContact(), new Elements(GetCurrentSwordElemental()), 100f);
                }
            }
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Range = 5f;
        MaxAttackPhase = 4;
        ResetBasicAttacks();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public Transform GetEmitterPivot()
    {
        return EmitterPivot;
    }
    public void ResetBasicAttacks()
    {
        BasicAttackPhase = 0;

        for (int i = 1; i <= MaxAttackPhase; i++)
        {
            string AtkName = "Attack" + i;
            if (ContainsParam(Animator, AtkName))
                Animator.SetBool(AtkName, false);
        }

        if (ContainsParam(Animator, "NextAtk"))
            Animator.SetBool("NextAtk", false);
    }

    protected override Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = base.PlungeAttackGroundHit(HitPos);
        foreach (Collider collider in colliders)
        {
            IDamage damageObject = collider.gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(collider.transform.position, new Elements(CurrentElement), GetCharacterData().GetDamage());
            }
        }
        return colliders;
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanAttack() && GetPlayerManager().GetPlayerMovementState() is not PlayerDashState)
            return;

        if (!GetModel().activeSelf)
            return;

        if (Time.time - LastClickedTime >= 1.15f)
        {
            ResetBasicAttacks();
        }

        if (Time.time - LastClickedTime > AttackRate && !Animator.GetBool("Attack" + MaxAttackPhase))
        {
            if (NearestEnemy != null)
            {
                Vector3 forward = NearestEnemy.transform.position - transform.position;
                forward.Normalize();
                LookAtDirection(forward);
            }

            BasicAttackPhase++;
            if (BasicAttackPhase > MaxAttackPhase)
            {
                ResetBasicAttacks();
            }

            string AtkName = "Attack" + BasicAttackPhase;
            if (ContainsParam(Animator, AtkName))
                Animator.SetBool(AtkName, true);

            LastClickedTime = Time.time + AttackRate;
        }

    }

    public int GetBasicAttackPhase()
    {
        return BasicAttackPhase;
    }

    public void SetAttackPhase(int phase)
    {
        BasicAttackPhase = phase;
    }

    public Elemental GetCurrentSwordElemental()
    {
        return CurrentElement;
    }
}
