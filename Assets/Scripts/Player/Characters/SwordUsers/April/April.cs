using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class April : SwordCharacters
{
    [SerializeField] GameObject ShieldPrefab;
    private float CoodinateTimerElapsed;
    private ParticleSystem Shield;

    protected override void Start()
    {
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

    public override void UpdateCoordinateAttack()
    {
        CoodinateTimerElapsed -= Time.deltaTime;
        CoodinateTimerElapsed = Mathf.Clamp(CoodinateTimerElapsed, 0f, GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer);

        if (Shield)
        {
            if (IsDead())
            {
                Destroy(Shield.gameObject);
            }
            else
            {
                Shield.transform.position = GetPlayerManager().transform.position;
            }
        }
    }

    public override bool CoordinateAttackEnded()
    {
        if (IsDead())
            return true;

        return CoodinateTimerElapsed <= 0f;
    }

    public override bool CoordinateCanShoot()
    {
        return false;
    }
}
