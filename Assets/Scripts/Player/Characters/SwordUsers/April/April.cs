using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class April : SwordCharacters
{
    [SerializeField] GameObject ShieldPrefab;
    [SerializeField] GameObject ShieldExplosion;
    [SerializeField] GameObject HealPrefab;
    private List<ParticleSystem> ExplosionShield;
    private float CoodinateTimerElapsed;
    private float nextHitExplosion;
    private ParticleSystem Shield;

    protected override void Start()
    {
        ExplosionShield = new();
        PlayerCharacterState = new AprilState(this);
        base.Start();
    }


    public void SpawnShield()
    {
        CoodinateTimerElapsed = GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer;
        if (Shield == null)
            Shield = Instantiate(ShieldPrefab).GetComponent<ParticleSystem>();
        Destroy(Shield.gameObject, CoodinateTimerElapsed);
    }

    private void DamageEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerManager().GetPlayerOffsetPosition().position, 2.8f, LayerMask.GetMask("Entity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage dmg = colliders[i].GetComponent<IDamage>();
            if (dmg != null)
            {
                if (!dmg.IsDead())
                    dmg.TakeDamage(dmg.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), 50);
            }
        }
        ParticleSystem ps = Instantiate(ShieldExplosion, GetPlayerManager().GetPlayerOffsetPosition().position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
        ExplosionShield.Add(ps);
        GetPlayerManager().HealCharacter(GetPlayerManager().GetCurrentCharacter().GetCharacterData(), 0.03f * GetMaxHealth());
        nextHitExplosion = Time.time;
    }

    public void DrainHealth()
    {
        if (GetHealth() > GetHealth() * 0.2f)
            SetHealth(GetHealth() - (GetHealth() * 0.3f));
    }

    public override void UpdateCoordinateAttack()
    {
        CoodinateTimerElapsed -= Time.deltaTime;
        CoodinateTimerElapsed = Mathf.Clamp(CoodinateTimerElapsed, 0f, GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer);

        for (int i = ExplosionShield.Count - 1; i > 0; i--)
        {
            if (ExplosionShield[i] == null)
                ExplosionShield.RemoveAt(i);
            else
                ExplosionShield[i].transform.position = GetPlayerManager().GetPlayerOffsetPosition().position;
        }

        if (Shield)
        {
            if (IsDead())
            {
                Destroy(Shield.gameObject);
            }
            else
            {
                if (CoordinateCanShoot())
                {
                    DamageEnemies();
                }
                Shield.transform.position = GetPlayerManager().transform.position;
            }
        }
    }

    public void SpawnHeal()
    {
        ParticleSystem heal = Instantiate(HealPrefab, GetPlayerManager().transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(heal.gameObject, heal.main.duration);
    }

    public override bool CoordinateAttackEnded()
    {
        if (IsDead())
            return true;

        return CoodinateTimerElapsed <= 0f;
    }

    public override bool CoordinateCanShoot()
    {
        return Time.time - nextHitExplosion > 1.5f;
    }
}
