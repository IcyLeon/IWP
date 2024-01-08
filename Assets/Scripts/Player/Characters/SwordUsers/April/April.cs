using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class April : SwordCharacters
{
    [SerializeField] GameObject ShieldPrefab;
    [SerializeField] GameObject ShieldExplosion;
    [SerializeField] GameObject HealPrefab;
    [SerializeField] GameObject EnemyMarkerPrefab;
    [SerializeField] LineRenderer ConnectionPrefab;
    private List<ParticleSystem> ExplosionShield;
    private float ShieldTimerElapsed;
    private float nextHitExplosion;
    private ParticleSystem Shield;

    protected override void Start()
    {
        ExplosionShield = new();
        PlayerCharacterState = new AprilState(this);
        base.Start();
        UltiRange = 4f;
    }

    public MarkerOnTargets GetEnemyMarker()
    {
        MarkerOnTargets go = Instantiate(EnemyMarkerPrefab).GetComponent<MarkerOnTargets>();
        return go;
    }

    public LineRenderer GetConnectionLine()
    {
        LineRenderer go = Instantiate(ConnectionPrefab).GetComponent<LineRenderer>();
        return go;
    }

    private AprilState GetAprilState()
    {
        return (AprilState)PlayerCharacterState;
    }

    public void SpawnShield()
    {
        ShieldTimerElapsed = GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer;
        if (Shield == null)
            Shield = Instantiate(ShieldPrefab).GetComponent<ParticleSystem>();
        Destroy(Shield.gameObject, ShieldTimerElapsed);
    }

    private void DamageEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerManager().GetPlayerOffsetPosition().position, 2.8f, LayerMask.GetMask("Entity", "BossEntity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage dmg = colliders[i].GetComponent<IDamage>();
            if (dmg != null)
            {
                if (!dmg.IsDead())
                    dmg.TakeDamage(dmg.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), 50, this);
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

    public override void UpdateISkills()
    {
        ShieldTimerElapsed -= Time.deltaTime;
        ShieldTimerElapsed = Mathf.Clamp(ShieldTimerElapsed, 0f, GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer);

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
                if (Time.time - nextHitExplosion > 1.5f)
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

    public override bool ISkillsEnded()
    {
        if (IsDead())
            return true;

        return ShieldTimerElapsed <= 0f;
    }

    public override bool IBurstEnded()
    {
        if (IsDead())
            return true;

        return false;
    }

    private void SetMarkersOnEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().GetPlayerOffsetPosition().position, GetPlayerCharacterState().GetPlayerCharacters().GetUltiRange(), LayerMask.GetMask("Entity", "BossEntity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage dmg = colliders[i].GetComponent<IDamage>();
            if (dmg != null)
            {
                MarkerEnemyData m = new MarkerEnemyData(GetEnemyMarker(), GetPlayerCharacterState().GetPlayerCharacters().GetCharacterData(), dmg, GetPlayersSO().ElementalBurstTimer);
                GetAprilState().aprilData.m_MarkerData.AddMarkerToEnemy(m);
            }
        }
    }

    public override void OnEntityHitSendInfo(Elements e, IDamage d)
    {
        MarkerEnemyData m = GetAprilState().aprilData.m_MarkerData.GetMarkerEnemyData(d);
        if (m != null)
        {
            if (GetPlayerManager().GetAliveCharacters() != null)
                GetPlayerManager().HealCharacter(GetPlayerManager().GetCurrentCharacter().GetCharacterData(), GetDEF() * 0.35f);

            if (e.GetElements() != Elemental.NONE)
            {
                DealDamageToAllConnected(e, m);
            }
        }
    }

    private void DealDamageToAllConnected(Elements e, MarkerEnemyData m)
    {
        foreach(var d in GetAprilState().aprilData.m_MarkerData.GetMarkerEnemyDataList())
        {
            if (d.Value != null)
                if (!d.Value.IsDead() && d.Value != m.GetIDamageObj())
                    d.Value.TakeDamage(d.Value.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), 0.1f * GetMaxHealth() + 95f, this, false);
        }
    }

    public void DestroyLine()
    {
        Destroy(GetAprilState().aprilData.connectedLineRenderer.gameObject);
    }

    public override void UpdateIBursts()
    {
        GetAprilState().aprilData.UpdateMarkerData();
        GetAprilState().aprilData.UpdateConnections();
    }

    public override void UpdateEveryTime()
    {
        if (IBurstEnded())
            GetAprilState().aprilData.RemoveAllMarkers();
    }
}
