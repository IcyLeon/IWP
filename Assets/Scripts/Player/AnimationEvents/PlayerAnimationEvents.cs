using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] FootStepSO FootStepSO;
    protected PlayerCharacters playerCharacters;

    private void OnFootstep()
    {
        if (playerCharacters.GetPlayerManager().GetPlayerState().GetPlayerMovementState() is not PlayerMovingState)
            return;

        SoundEffectsManager.GetInstance().PlaySFXSound(FootStepSO.GetRandomFootstepSound(), 1, playerCharacters.GetPointOfContact(), 1f);
    }

    private void OnLand()
    {
        SoundEffectsManager.GetInstance().PlaySFXSound(FootStepSO.GetRandomFootstepSound(), 1, playerCharacters.GetPointOfContact(), 1f);
    }

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
        if (playerCharacters == null)
            return;

        Animator animator = playerCharacters.GetAnimator();
        PlayerManager PM = playerCharacters.GetPlayerManager();

        if (animator == null)
            return;

        if (PM == null)
            return;

        if (!PM.GetPlayerMovementState().CheckIfisAboutToFall())
        {
            PM.GetCharacterRB().transform.position += animator.deltaPosition;
        }
        playerCharacters.GetPlayerManager().GetCharacterRB().transform.rotation = animator.rootRotation;
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
    }
}
