using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private SwordCharacters SwordCharacters;
    private Dictionary<Collider, bool> ChargeColliderList = new();
    [SerializeField] SwordSoundSO SwordSoundSO;
    [SerializeField] Transform SlashPivot;
    

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
        if (SwordSoundSO != null)
        {
            SwordCharacters.GetPlayerSoundManager().PlaySound(SwordSoundSO.GetRandomSwingClip());
        }
        ChargeColliderList.Clear();
    }
    private void OnTriggerStay(Collider other)
    {
        if (SwordCharacters.GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState() is not PlayerAttackState)
            return;

        if (!ChargeColliderList.TryGetValue(other, out bool value))
        {
            IDamage damageObj = other.GetComponent<IDamage>();
            if (damageObj != null)
            {
                if (!damageObj.IsDead())
                {
                    Vector3 hitPosition = other.ClosestPointOnBounds(transform.position);

                    ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, hitPosition, Quaternion.identity).GetComponent<ParticleSystem>();
                    Destroy(hitEffect.gameObject, hitEffect.main.duration);
                    damageObj.TakeDamage(damageObj.GetPointOfContact(), new Elements(SwordCharacters.GetCurrentSwordElemental()), SwordCharacters.GetATK(), SwordCharacters);
                    SwordCharacters.GetPlayerManager().GetPlayerController().GetCameraManager().CameraShake(2f, 2f, 0.15f);


                    if (SwordSoundSO != null)
                    {
                        SwordCharacters.GetPlayerSoundManager().PlaySound(SwordSoundSO.GetRandomHitClip());
                    }
                }

                ChargeColliderList.Add(other, true);
            }
        }
    }
}
