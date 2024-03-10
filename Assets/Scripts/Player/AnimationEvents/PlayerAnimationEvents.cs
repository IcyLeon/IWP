using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    protected PlayerCharacters playerCharacters;

    private void PlayRandomFallenVoice()
    {
        playerCharacters.PlayRandomFallenVoice();
    }

    private void PlayRandomBasicAttackVoice()
    {
        playerCharacters.PlayRandomBasicAttackVoice();
    }

    private void OnAnimatorMove()
    {
        if (playerCharacters.GetAnimator() == null)
        {
            return;
        }

        playerCharacters.AnimatorMove(playerCharacters.GetAnimator().deltaPosition, playerCharacters.GetAnimator().rootRotation);
    }

    protected virtual void Awake()
    {
        playerCharacters = transform.parent.GetComponent<PlayerCharacters>();
    }

    public void TriggerOnAnimationTransition()
    {
        playerCharacters.GetPlayerManager().OnAnimationTransition();
    }

    private void StopBurstAnimation()
    {
        if (playerCharacters.GetBurstCamera())
        {
            playerCharacters.GetBurstCamera().gameObject.SetActive(false);
            playerCharacters.GetPlayerManager().GetPlayerController().GetCameraManager().Recentering();
        }
    }

    public void TriggerOnCharacterStateAnimationTransition()
    {
        playerCharacters.GetPlayerCharacterState().OnAnimationTransition();
    }

    protected virtual void Attack()
    {
        PlayerElementalSkillandBurstManager.CallCoordinateAttack();

        if (playerCharacters)
            playerCharacters.SetisAttacking(false);
    }
}
