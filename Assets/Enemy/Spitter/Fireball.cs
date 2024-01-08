using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private IDamage source;

    private void Start()
    {
        StartCoroutine(Timeout());
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(5f);
        Explode();
    }

    public void SetSource(IDamage source)
    {
        this.source = source;
    }
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Player"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage damageObject = colliders[i].gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), new Elements(Elemental.FIRE), 100f, source);

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
