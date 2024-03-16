using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAreaOfEffect : MonoBehaviour
{
    private float FireInterval = 0.45f, FireIntervalElasped = 0;
    private MageEnemy MageEnemy;
    private float Timer = 180;

    public void Init(MageEnemy m)
    {
        MageEnemy = m;
    }

    // Update is called once per frame
    void Update()
    {
        DamageOverTime();
    }

    private void DamageOverTime()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        if (Time.time - FireIntervalElasped > FireInterval)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f, LayerMask.GetMask("Player", "FF"));
            for (int i = 0; i < colliders.Length; i++)
            {
                IDamage damage = colliders[i].GetComponent<IDamage>();
                if (damage != null)
                {
                    damage.TakeDamage(damage.GetPointOfContact(), new Elements(Elemental.FIRE), MageEnemy.GetATK() * 0.4f, MageEnemy);
                }
            }
            FireIntervalElasped = Time.time;
        }
    }
}
