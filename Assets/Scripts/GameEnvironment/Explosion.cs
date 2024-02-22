using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem ExplosionPS;
    private SphereCollider Collider;
    private LayerMask LayerMask;
    private float dmg;
    private IDamage source;
    private Elemental elemental;
    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
        ExplosionPS = GetComponent<ParticleSystem>();
    }

    public void SetExplosionSound(AudioClip c)
    {
        audioSource.PlayOneShot(c);
    }
    public void Init(Elemental e, LayerMask LayerMask, float explosionRadius, float damage, IDamage source)
    {
        this.LayerMask = LayerMask;
        elemental = e;
        dmg = damage;
        this.source = source;
        Collider.radius = explosionRadius;
        StartCoroutine(WaitAfterSoundIsFinished());
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the layer of the collided object is within the specified LayerMask
        if (LayerMask == (LayerMask | (1 << other.gameObject.layer)))
        {
            IDamage damageObject = other.gameObject.GetComponent<IDamage>();
            if (damageObject != null && !damageObject.IsDead())
            {
                Vector3 hitPosition = other.ClosestPointOnBounds(transform.position);
                damageObject.TakeDamage(hitPosition, new Elements(elemental), dmg, source);
            }
        }
    }

    private IEnumerator WaitAfterSoundIsFinished()
    {
        yield return null;
        Collider.enabled = false;
        yield return new WaitForSeconds(ExplosionPS.main.duration);
        Destroy(gameObject);
    }
}
