using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] AudioClip explosionClipSound;
    private Elemental elemental;
    private IDamage source;

    private void Start()
    {
        StartCoroutine(Timeout());
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(5f);
        Explode();
        Destroy(gameObject);
    }

    public void SetSource(IDamage source)
    {
        this.source = source;
    }

    public void SetElement(Elemental e)
    {
        elemental = e;
    }

    void Explode()
    {
        Explosion explosion = Instantiate(AssetManager.GetInstance().HitExplosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
        explosion.SetExplosionSound(explosionClipSound);
        explosion.Init(elemental, LayerMask.GetMask("Player"), 2.5f, 100f, source);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        BaseEnemy BaseEnemy = other.GetComponent<BaseEnemy>();

        if (BaseEnemy == null)
        {
            Explode();
            Destroy(gameObject);
        }
    }
}
