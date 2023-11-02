using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCharacters : PlayerCharacters
{
    [SerializeField] protected GameObject SwordModel;
    [SerializeField] protected Transform EmitterPivot;
    protected int BasicAttackPhase;
    protected Elemental CurrentElement;
    protected int AttackLayer;

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
        Animator.SetInteger("AttackPhase", BasicAttackPhase);
    }

    protected override Collider[] PlungeAttackGroundHit()
    {
        Collider[] colliders = base.PlungeAttackGroundHit();
        foreach (Collider collider in colliders)
        {
            IDamage damageObject = collider.gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                damageObject.TakeDamage(collider.transform.position, new Elements(CurrentElement), GetCharacterData().GetDamage());
            }
        }
        return colliders;
    }

    protected override void ChargeTrigger()
    {
        if (GetPlayerController().GetPlayerActionStatus() != PlayerActionStatus.IDLE)
            return;

        if (BasicAttackPhase > 2)
        {
            ResetBasicAttacks();
        }

        Animator.SetInteger("AttackPhase", BasicAttackPhase);
        Animator.SetTrigger("Attack");
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
