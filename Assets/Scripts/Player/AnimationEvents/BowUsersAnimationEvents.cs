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

    protected override void Attack()
    {
        BowData bowData = GetBowCharacters().GetBowCharactersState().GetBowData();
        Arrow ArrowFire = Instantiate(ArrowPrefab, GetBowCharacters().GetEmitterPivot().transform).GetComponent<Arrow>();
        ArrowFire.SetElements(new Elements(GetBowCharacters().GetBowCharactersState().GetBowData().ShootElemental));
        ArrowFire.SetCharacterData(GetBowCharacters());
        ArrowFire.transform.SetParent(null);

        ParticleSystem arrowLuanch = Instantiate(AssetManager.GetInstance().ArrowLaunchPrefab, GetBowCharacters().GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(arrowLuanch.gameObject, arrowLuanch.main.duration);

        if (!GetBowCharacters().GetPlayerManager().IsAiming())
            GetBowCharacters().GetSoundManager().PlaySFXSound(GetBowCharacters().GetBowSoundSO().GetRandomBasicFireAudioClip());
        else
            GetBowCharacters().GetSoundManager().PlaySFXSound(GetBowCharacters().GetBowSoundSO().AimFireAudioClip);

        ArrowFire.GetRigidbody().velocity = bowData.Direction.normalized * BowData.BaseFireSpeed * (1 + GetBowCharacters().GetBowCharactersState().GetBowData().ChargeElapsed);

        GetBowCharacters().DestroyChargeUpEmitter();
        GetBowCharacters().GetBowCharactersState().GetBowData().ShootElemental = Elemental.NONE;

        base.Attack();
    }
}
