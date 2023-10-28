using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacters : Characters
{
    protected float AimSpeed = 20f;
    private CharacterData characterData;
    private PlayerController playerController;
    private Coroutine CameraZoomAndPosOffsetCoroutine;

    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    public PlayerCharacterSO GetPlayersSO()
    {
        return CharactersSO as PlayerCharacterSO;
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public void Attack()
    {

    }


    public Characters GetNearestCharacters()
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerController().transform.position, 7.5f, LayerMask.GetMask("Entity"));
        if (colliders.Length == 0)
            return null;

        Collider CurrentCollider = colliders[0];

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            float Dist1 = (collider.transform.position - GetPlayerController().transform.position).magnitude;
            float Dist2 = (collider.transform.position - CurrentCollider.transform.position).magnitude;
            if (Dist1 < Dist2)
                CurrentCollider = collider;
        }
        return CurrentCollider.GetComponent<Characters>();
    } 

    public override int GetLevel()
    {
        if (characterData == null)
            return -1;

        return characterData.GetLevel();
    }

    public override float GetHealth()
    {
        if (characterData == null)
            return 1;

        return characterData.GetHealth();
    }

    public override float GetMaxHealth()
    {
        if (characterData == null)
            return base.GetMaxHealth();

        return characterData.GetMaxHealth();
    }

    public override float GetDamage()
    {
        if (characterData == null)
            return base.GetDamage();

        return characterData.GetDamage();
    }

    public override void SetHealth(float val)
    {
        if (characterData == null)
            return;

        characterData.SetHealth(val);
    }

    public override float GetDEF()
    {
        if (characterData == null)
            return base.GetDEF();

        return characterData.GetDEF();
    }

    protected PlayerController GetPlayerController()
    {
        return playerController;
    }
    protected override void Start()
    {
        playerController = CharacterManager.GetInstance().GetPlayerController();
        GetPlayerController().OnElementalSkillHold += ElementalSkillHold;
        GetPlayerController().OnElementalBurstTrigger += ElementalBurstTrigger;
        GetPlayerController().OnElementalSkillTrigger += ElementalSkillTrigger;
        GetPlayerController().OnE_1Down += EKey_1Down;
        GetPlayerController().OnChargeHold += ChargeHold;
        GetPlayerController().OnChargeTrigger += ChargeTrigger;
        GetPlayerController().onPlayerStateChange += OnJump;

        healthBarScript = MainUI.GetInstance().GetPlayerHealthBar();
        base.Start();
    }

    private IEnumerator UpdateDefaultPosOffsetAndZoomAnim(float delay)
    {
        if (GetPlayerController() == null)
            yield break;

        yield return new WaitForSeconds(delay);
        GetPlayerController().UpdateDefaultPosOffsetAndZoom();
        yield return null;
    }

    public void UpdateDefaultPosOffsetAndZoom(float delay)
    {
        if (CameraZoomAndPosOffsetCoroutine != null)
            StopCoroutine(CameraZoomAndPosOffsetCoroutine);

        if (gameObject.activeSelf)
            CameraZoomAndPosOffsetCoroutine = StartCoroutine(UpdateDefaultPosOffsetAndZoomAnim(delay));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public CinemachineVirtualCamera GetVirtualCamera()
    {
        if (GetPlayerController() == null)
            return null;

        return GetPlayerController().GetVirtualCamera();
    }

    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (GetPlayerController() == null)
            return Vector3.zero;

        return GetPlayerController().GetRayPosition3D(origin, direction, maxdistance);
    }

    protected void UpdateInputTargetQuaternion()
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().UpdateInputTargetQuaternion();
    }

    protected void SetTargetRotation(Quaternion quaternion)
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().SetTargetRotation(quaternion);
    }

    protected void LookAtDirection(Vector3 dir)
    {
        Quaternion quaternion = Quaternion.LookRotation(dir);
        SetTargetRotation(quaternion);
    }
    protected void ResetVelocity()
    {
        if (GetPlayerController() == null)
            return;
        GetPlayerController().ResetVelocity();
    }

    protected void UpdateCameraAim()
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().UpdateAim();
    }

    protected virtual void ElementalSkillTrigger()
    {
    }

    protected virtual void ElementalSkillHold()
    {
    }

    protected virtual void EKey_1Down()
    {

    }
    protected virtual void ElementalBurstTrigger()
    {

    }
    protected virtual void ChargeHold()
    {

    }
    protected virtual void ChargeTrigger()
    {

    }

    protected virtual void OnDestroy()
    {
        if (GetPlayerController() != null)
        {
            GetPlayerController().OnElementalSkillHold -= ElementalSkillHold;
            GetPlayerController().OnElementalBurstTrigger -= ElementalBurstTrigger;
            GetPlayerController().OnElementalSkillTrigger -= ElementalSkillTrigger;
            GetPlayerController().OnE_1Down -= EKey_1Down;
            GetPlayerController().OnChargeHold -= ChargeHold;
            GetPlayerController().OnChargeTrigger -= ChargeTrigger;
            GetPlayerController().onPlayerStateChange -= OnJump;
        }
    }

    private void OnJump(PlayerActionStatus state)
    {
        if (state != PlayerActionStatus.JUMP)
            return;

        if (Animator != null)
            Animator.SetTrigger("Jump");
    }
}
