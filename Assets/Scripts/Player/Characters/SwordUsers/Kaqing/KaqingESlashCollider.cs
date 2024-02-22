using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingESlashCollider : MonoBehaviour
{
    private Kaqing Kaqing;
    [SerializeField] AudioClip audioClip;
    [SerializeField] Collider Collider;
    private void Awake()
    {
        Collider.enabled = false;
    }

    public void SetKaqing(Kaqing Kaqing)
    {
        this.Kaqing = Kaqing;
    }

    public void SetCanHit(bool val)
    {
        Collider.enabled = val;
    }


    private void OnTriggerEnter(Collider other)
    {
        FriendlyKillers friendlyKillers = other.GetComponent<FriendlyKillers>();
        PlayerCharacters pc = other.GetComponent<PlayerCharacters>();

        if (friendlyKillers != null)
            return;

        if (pc != null)
            return;

        IDamage damageObj = other.GetComponent<IDamage>();
        if (damageObj != null)
        {
            if (!damageObj.IsDead())
            {
                Vector3 hitPosition = other.ClosestPointOnBounds(transform.position);

                ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, hitPosition, Quaternion.identity).GetComponent<ParticleSystem>();
                Destroy(hitEffect.gameObject, hitEffect.main.duration);
                damageObj.TakeDamage(hitPosition, new Elements(Kaqing.GetCurrentSwordElemental()), Kaqing.GetATK() * 2f, Kaqing);

                if (Kaqing.GetSwordModel().GetSwordSoundSO() != null)
                {
                    Kaqing.GetSoundManager().PlaySFXSound(Kaqing.GetSwordModel().GetSwordSoundSO().GetRandomHitClip());
                }
            }

        }
    }
}
