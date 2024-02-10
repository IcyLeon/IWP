using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private SwordCharacters SwordCharacters;
    private Dictionary<Collider, bool> ChargeColliderList = new();
    [SerializeField] SwordSoundSO SwordSoundSO;
    [SerializeField] Transform SlashPivot;
    [SerializeField] Collider Collider;

    private void Awake()
    {
        Collider.enabled = false;
    }

    public void SetCanHit(bool val)
    {
        Collider.enabled = val;
        if (!Collider.enabled)
        {
            ChargeColliderList.Clear();
        }
    }
    public SwordSoundSO GetSwordSoundSO()
    {
        return SwordSoundSO;
    }
    public Transform GetSlashPivot()
    {
        return SlashPivot;
    }
    public void SetPlayerCharacter(SwordCharacters SwordCharacters)
    {
        this.SwordCharacters = SwordCharacters;
    }

    public void ResetHits()
    {
        if (GetSwordSoundSO() != null)
        {
            SwordCharacters.GetSoundManager().PlaySFXSound(GetSwordSoundSO().GetRandomSwingClip());
        }
        ChargeColliderList.Clear();
    }
    private void OnTriggerStay(Collider other)
    {
        if (SwordCharacters.GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState() is not PlayerAttackState)
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
                    damageObj.TakeDamage(damageObj.GetPointOfContact(), new Elements(SwordCharacters.GetCurrentSwordElemental()), SwordCharacters.GetATK(), SwordCharacters);
                    SwordCharacters.GetPlayerManager().GetPlayerController().GetCameraManager().CameraShake(2f, 2f, 0.15f);


                    if (GetSwordSoundSO() != null)
                    {
                        SwordCharacters.GetSoundManager().PlaySFXSound(GetSwordSoundSO().GetRandomHitClip());
                    }
                }

                ChargeColliderList.Add(other, true);
            }
        }
    }
}
