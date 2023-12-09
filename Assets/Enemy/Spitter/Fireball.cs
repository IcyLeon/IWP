using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Player"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage damageObject = colliders[i].gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), new Elements(Elemental.FIRE), 100f);

                //Debug.Log(damageObject);
            }
        }

        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().HitExplosion, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(hitEffect.gameObject, hitEffect.main.duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseEnemy BaseEnemy = other.GetComponent<BaseEnemy>();

        if (BaseEnemy == null)
        {
            Explode();
            Destroy(gameObject);
        }
    }
}
