using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordCharacters : PlayerCharacters
{
    [SerializeField] protected GameObject SwordModel;
    [SerializeField] protected Transform EmitterPivot;
    protected int BasicAttackPhase = 0;
    protected Elemental CurrentElement;
    private float LastClickedTime, AttackRate = 0.3f;
    protected int AttackLayer;

    public void SpawnSlash()
    {
        AssetManager.GetInstance().SpawnSlashEffect(GetEmitterPivot());
        Collider[] Colliders = Physics.OverlapSphere(GetPlayerController().GetPlayerOffsetPosition().position + Vector3.up + transform.forward * 2f, 2f, LayerMask.GetMask("Entity"));
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
                    damageObj.TakeDamage(hitPosition, new Elements(GetCurrentSwordElemental()), 100f);
                }
            }
        }

    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(GetPlayerController().GetPlayerOffsetPosition().position + transform.forward * 2f, 2f);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
        if (GetPlayerController().GetPlayerGroundStatus() != PlayerGroundStatus.GROUND || !GetPlayerController().IsInMovingState())
            return;

        if (GetBurstActive() || !GetModel().activeSelf)
            return;

        if (Time.time - LastClickedTime >= 1.5f)
        {
            ResetBasicAttacks();
        }

        if (Time.time - LastClickedTime > AttackRate && !Animator.GetCurrentAnimatorStateInfo(1).IsName("Attack4"))
        {
            if (NearestEnemy != null)
            {
                Vector3 forward = NearestEnemy.transform.position - transform.position;
                forward.Normalize();
                LookAtDirection(forward);
            }

            BasicAttackPhase++;
            if (BasicAttackPhase > 4)
            {
                ResetBasicAttacks();
            }
            SetisAttacking(true);
            Animator.SetTrigger("Attack" + BasicAttackPhase);
            LastClickedTime = Time.time;
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
