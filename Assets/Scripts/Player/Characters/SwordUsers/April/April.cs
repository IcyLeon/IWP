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
    [SerializeField] LineRenderer ConnectionPrefab;
    private float ShieldTimerElapsed;
    private float nextHitExplosion;
    private ParticleSystem Shield;

    protected override void Start()
    {
        PlayerCharacterState = new AprilState(this);
        base.Start();
        UltiRange = 4f;
    }

    public LineRenderer GetConnectionLine()
    {
        LineRenderer go = Instantiate(ConnectionPrefab).GetComponent<LineRenderer>();
        return go;
    }

    public AprilState GetAprilState()
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
        Collider[] colliders = Physics.OverlapSphere(GetPointOfContact(), 2.8f, LayerMask.GetMask("Entity", "BossEntity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage dmg = colliders[i].GetComponent<IDamage>();
            if (dmg != null)
            {
                if (!dmg.IsDead())
                {
                    Vector3 hitPosition = colliders[i].ClosestPointOnBounds(GetPlayerManager().GetPlayerOffsetPosition().position);
                    dmg.TakeDamage(hitPosition, new Elements(GetPlayersSO().Elemental), 50, this);
                }
            }
        }
        ParticleSystem ps = Instantiate(ShieldExplosion, GetPlayerManager().GetPlayerOffsetPosition().position, Quaternion.identity).GetComponent<ParticleSystem>();
        ps.transform.SetParent(GetPlayerManager().transform, true);

        Destroy(ps.gameObject, ps.main.duration);
        GetPlayerManager().HealCharacter(GetPlayerManager().GetCurrentCharacter().GetCharacterData(), 0.03f * GetMaxHealth());
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

        //for (int i = ExplosionShield.Count - 1; i > 0; i--)
        //{
        //    if (ExplosionShield[i] == null)
        //        ExplosionShield.RemoveAt(i);
        //    else
        //        ExplosionShield[i].transform.position = GetPointOfContact();
        //}

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
                    nextHitExplosion = Time.time;
                }
            }

            Shield.transform.position = GetPlayerManager().GetPlayerOffsetPosition().position;
        }
    }

    public override bool IsISkillsEnded()
    {
        return base.IsISkillsEnded();
    }

    public override bool IsIBurstEnded()
    {
        return base.IsIBurstEnded();
    }

    public override void OnEntityHitSendInfo(ElementalReactionsTrigger ER, Elements e, IDamage d)
    {
        MarkerEnemyData m = GetAprilState().aprilData.m_MarkerData.GetMarkerEnemyData(d);
        if (m != null)
        {
            if (GetPlayerManager().GetAliveCharacters() != null)
                GetPlayerManager().HealCharacter(GetPlayerManager().GetCurrentCharacter().GetCharacterData(), 25f + GetDEF() * 0.35f);

            if (ER != null)
            {
                DealDamageToAllConnected(e, m);
            }
        }
    }

    private void DealDamageToAllConnected(Elements e, MarkerEnemyData m)
    {
        foreach(var d in GetAprilState().aprilData.m_MarkerData.GetMarkerEnemyDataList())
        {
            if (d.Key != null)
                if (!d.Key.IsDead() && d.Value != m.GetIDamageObj())
                    d.Key.TakeDamage(d.Key.GetPointOfContact(), new Elements(GetPlayersSO().Elemental), 0.15f * GetMaxHealth() + 35f, this, false);
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
        if (IsIBurstEnded())
            GetAprilState().aprilData.RemoveAllMarkers();
    }
}
