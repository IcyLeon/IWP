using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordCharacters : PlayerCharacters
{
    [SerializeField] protected Sword SwordModel;
    protected int BasicAttackPhase = 0;
    private int MaxAttackPhase;
    protected Elemental CurrentElement;
    private float LastClickedTime, AttackRate = 0.08f;

    public Sword GetSwordModel()
    {
        return SwordModel;
    }
    public virtual void SpawnSlash()
    {
        BasicAttackTrigger();
        AssetManager.GetInstance().SpawnSlashEffect(GetSwordModel().GetSlashPivot());
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
                    damageObj.TakeDamage(damageObj.GetPointOfContact(), new Elements(GetCurrentSwordElemental()), GetATK());
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


    public void ResetBasicAttacks()
    {
        BasicAttackPhase = 0;

        for (int i = 1; i <= MaxAttackPhase; i++)
        {
            string AtkName = "Attack" + i;
            if (ContainsParam(Animator, AtkName))
                Animator.ResetTrigger(AtkName);
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
                    damageObject.TakeDamage(collider.transform.position, new Elements(CurrentElement), GetCharacterData().GetATK());
            }
        }
        return colliders;
    }

    public void SpawnHitEffect(IDamage damage)
    {
        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, damage.GetPointOfContact(), Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(hitEffect.gameObject, hitEffect.main.duration);
        damage.TakeDamage(damage.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), GetATK());
    }

    public override void LaunchBasicAttack()
    {
        if (Time.time - LastClickedTime >= 1f)
        {
            ResetBasicAttacks();
        }


        if (Time.time - LastClickedTime > AttackRate && BasicAttackPhase < MaxAttackPhase && !GetPlayerManager().IsSkillCasting())
        {
            SetLookAtTarget();
            BasicAttackPhase++;
            if (BasicAttackPhase > MaxAttackPhase)
            {
                ResetBasicAttacks();
            }

            string AtkName = "Attack" + BasicAttackPhase;
            if (ContainsParam(Animator, AtkName))
                Animator.SetTrigger(AtkName);

            LastClickedTime = Time.time + AttackRate;
        }
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanPerformAction())
            return;

        if (!GetModel().activeSelf)
            return;

        base.ChargeTrigger();
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
