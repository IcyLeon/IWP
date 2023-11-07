using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordCharacters : PlayerCharacters
{
    [SerializeField] protected GameObject SwordModel;
    [SerializeField] protected Transform EmitterPivot;
    protected int BasicAttackPhase = -1;
    protected Elemental CurrentElement;
    private float LastClickedTime, AttackRate = 0.15f;
    protected int AttackLayer;

    public void SpawnSlash()
    {
        GameObject slash = AssetManager.GetInstance().SpawnSlashEffect(GetEmitterPivot());
        slash.GetComponentInChildren<Sword>().SetSwordCharacterWield(this);
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
        if (GetPlayerController().GetPlayerGroundStatus() != PlayerGroundStatus.GROUND || GetPlayerController().GetPlayerActionStatus() != PlayerActionStatus.IDLE)
            return;

        if (GetBurstActive())
            return;

        if (Time.time - LastClickedTime >= 1.5f)
        {
            ResetBasicAttacks();
        }

        if (Time.time - LastClickedTime > AttackRate && !Animator.GetCurrentAnimatorStateInfo(1).IsName("Attack3"))
        {
            BasicAttackPhase++;
            if (BasicAttackPhase > 4)
            {
                ResetBasicAttacks();
            }

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
