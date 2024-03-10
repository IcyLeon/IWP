using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordCharacters : PlayerCharacters
{
    [SerializeField] protected Sword SwordModel;
    protected Elemental CurrentElement;

    public Sword GetSwordModel()
    {
        return SwordModel;
    }

    public virtual void SpawnSlash()
    {
        AssetManager.GetInstance().SpawnSlashEffect(GetSwordModel().GetSlashPivot());
        GetSwordModel().ResetHits();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GetSwordModel().SetPlayerCharacter(this);
        Range = 5f;
    }

    protected override Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = base.PlungeAttackGroundHit(HitPos);

        if (GetCurrentSwordElemental() == Elemental.NONE)
            AssetManager.GetInstance().SpawnParticlesEffect(HitPos, AssetManager.GetInstance().PlungeParticlesEffect);

        foreach (Collider collider in colliders)
        {
            IDamage damageObject = collider.gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), new Elements(CurrentElement), GetATK() * 3.5f, this);
            }
        }
        return colliders;
    }

    public void SpawnHitEffect(IDamage damage)
    {
        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, damage.GetPointOfContact(), Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(hitEffect.gameObject, hitEffect.main.duration);
        damage.TakeDamage(damage.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), GetATK() * 1.8f, this);
        SoundEffectsManager.GetInstance().PlaySFXSound(GetSwordModel().GetSwordSoundSO().GetRandomHitClip());
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanPerformAction())
            return;

        if (!GetModel().activeSelf)
            return;

        base.ChargeTrigger();
    }

    public Elemental GetCurrentSwordElemental()
    {
        return CurrentElement;
    }
}
