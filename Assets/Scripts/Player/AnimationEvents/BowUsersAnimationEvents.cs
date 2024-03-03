using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowUsersAnimationEvents : PlayerAnimationEvents
{
    [SerializeField] GameObject ArrowPrefab;

    public BowCharacters GetBowCharacters()
    {
        return playerCharacters as BowCharacters;
    }

    private void FireArrows()
    {
        BowData bowData = GetBowCharacters().GetBowCharactersState().GetBowData();
        Arrow ArrowFire = Instantiate(ArrowPrefab, GetBowCharacters().GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<Arrow>();
        ParticleSystem arrowLuanch = Instantiate(AssetManager.GetInstance().ArrowLaunchPrefab, GetBowCharacters().GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(arrowLuanch.gameObject, arrowLuanch.main.duration);
        ArrowFire.SetElements(new Elements(GetBowCharacters().GetBowCharactersState().GetBowData().ShootElemental));
        ArrowFire.SetCharacterData(GetBowCharacters());
        ArrowFire.transform.rotation = Quaternion.LookRotation(bowData.Direction);

        if (!GetBowCharacters().GetPlayerManager().IsAiming())
            GetBowCharacters().GetSoundManager().PlaySFXSound(GetBowCharacters().GetBowSoundSO().GetRandomBasicFireAudioClip());
        else
            GetBowCharacters().GetSoundManager().PlaySFXSound(GetBowCharacters().GetBowSoundSO().AimFireAudioClip);

        ArrowFire.GetRigidbody().AddForce(bowData.Direction.normalized * BowData.BaseFireSpeed * (1 + GetBowCharacters().GetBowCharactersState().GetBowData().ChargeElapsed));

        GetBowCharacters().DestroyChargeUpEmitter();
        GetBowCharacters().GetBowCharactersState().GetBowData().ShootElemental = Elemental.NONE;
        GetBowCharacters().BasicAttackTrigger();
    }
}
