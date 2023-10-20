using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    public void TakeDamage(GameObject source);
}
public class Characters : MonoBehaviour, IDamage
{
    protected float AimSpeed = 20f;
    [SerializeField] PlayersSO PlayersSO;
    private CharacterData characterData;
    private PlayerController playerController;
    private Coroutine CameraZoomAndPosOffsetCoroutine;

    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    public PlayersSO GetPlayersSO()
    {
        return PlayersSO;
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public void Attack()
    {

    }

    public int GetLevel()
    {
        if (characterData == null)
            return -1;

        return characterData.GetLevel();
    }

    protected PlayerController GetPlayerController()
    {
        return playerController;
    }

    protected virtual void Start()
    {
        playerController = CharacterManager.GetInstance().GetPlayerController();
        GetPlayerController().OnElementalSkillHold += ElementalSkillHold;
        GetPlayerController().OnElementalBurstTrigger += ElementalBurstTrigger;
        GetPlayerController().OnElementalSkillTrigger += ElementalSkillTrigger;
        GetPlayerController().OnE_1Down += EKey_1Down;
        GetPlayerController().OnChargeHold += ChargeHold;
        GetPlayerController().OnChargeTrigger += ChargeTrigger;
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
    protected virtual void Update()
    {

    }

    public CinemachineVirtualCamera GetVirtualCamera()
    {
        if (GetPlayerController() == null)
            return null;

        return GetPlayerController().GetVirtualCamera();
    }

    protected void UpdateCameraOffsetPosition(float ModifierX, float ModifierY, float Speed)
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().CameraOffsetPositionAnim(ModifierX, ModifierY, Speed);
    }

    protected void UpdateCameraZoomOffset(float CameraDistance, float Speed)
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().CameraZoomOffsetAnim(CameraDistance, Speed);
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
        UpdateCameraOffsetPosition(0.25f, 0.5f, AimSpeed);
        UpdateCameraZoomOffset(3f, AimSpeed);
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

    private void OnDestroy()
    {
        if (GetPlayerController() != null)
        {
            GetPlayerController().OnElementalSkillHold -= ElementalSkillHold;
            GetPlayerController().OnElementalBurstTrigger -= ElementalBurstTrigger;
            GetPlayerController().OnElementalSkillTrigger -= ElementalSkillTrigger;
            GetPlayerController().OnE_1Down -= EKey_1Down;
            GetPlayerController().OnChargeHold -= ChargeHold;
            GetPlayerController().OnChargeTrigger -= ChargeTrigger;
        }
    }

    public void TakeDamage(GameObject source)
    {

    }
}
