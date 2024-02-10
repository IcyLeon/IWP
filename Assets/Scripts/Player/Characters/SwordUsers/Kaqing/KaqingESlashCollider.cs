using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingESlashCollider : MonoBehaviour
{
    private Kaqing Kaqing;
    private Dictionary<Collider, bool> ChargeColliderList = new();
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
        if (!Collider.enabled)
        {
            ChargeColliderList.Clear();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Kaqing.GetPlayerCharacterState().GetCurrentState() is not KaqingESlash)
            return;

        FriendlyKillers friendlyKillers = other.GetComponent<FriendlyKillers>();
        PlayerCharacters pc = other.GetComponent<PlayerCharacters>();


        if (friendlyKillers != null)
            return;

        if (pc != null)
            return;


        if (!ChargeColliderList.TryGetValue(other, out bool value))
        {
            IDamage damageObj = other.GetComponent<IDamage>();
            if (damageObj != null)
            {
                if (other.GetComponent<PlayerCharacters>() != null)
                    return;

                if (!damageObj.IsDead())
                {
                    Vector3 hitPosition = other.ClosestPointOnBounds(transform.position);

                    ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, hitPosition, Quaternion.identity).GetComponent<ParticleSystem>();
                    Destroy(hitEffect.gameObject, hitEffect.main.duration);
                    damageObj.TakeDamage(damageObj.GetPointOfContact(), new Elements(Kaqing.GetCurrentSwordElemental()), Kaqing.GetATK() * 2f, Kaqing);
                    Kaqing.GetPlayerManager().GetPlayerController().GetCameraManager().CameraShake(2f, 2f, 0.15f);

                    if (Kaqing.GetSwordModel().GetSwordSoundSO() != null)
                    {
                        Kaqing.GetSoundManager().PlaySFXSound(Kaqing.GetSwordModel().GetSwordSoundSO().GetRandomHitClip());
                    }
                }

                ChargeColliderList.Add(other, true);
            }
        }
    }
}
