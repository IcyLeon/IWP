using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    protected float AimSpeed = 20f;
    private CharacterData characterData;
    // Start is called before the first frame update
    protected List<Artifacts> EquippedArtifacts = new List<Artifacts>();
    private PlayerController playerController;
    private Coroutine CameraZoomAndPosOffsetCoroutine;
    [SerializeField] PlayersSO playersSO;

    public PlayersSO GetPlayersSO()
    {
        return playersSO;
    }

    public void Attack()
    {

    }

    public int GetLevel()
    {
        if (characterData == null)
            return 1;

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
    public List<Artifacts> GetEquippedArtifactsList()
    {
        return EquippedArtifacts;
    }

    public Artifacts CheckIfArtifactTypeExist(ArtifactType artifacttype)
    {
        for (int i = 0; i < EquippedArtifacts.Count; i++)
        {
            if (EquippedArtifacts[i].GetArtifactType() == artifacttype)
            {
                return EquippedArtifacts[i];
            }
        }
        return null;
    }
}
