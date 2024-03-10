using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class AimController : MonoBehaviour
{
    [SerializeField] protected PlayerCharacters PlayerCharactersRef;
    [SerializeField] Rig Rig;
    [SerializeField] Transform TargetTransform;
    private Vector3 PreviousAimLocation;
    private float Soothing = 10f;
    private float TargetWeight;

    void Awake()
    {
        SetTargetWeight(0f);
        Rig.weight = TargetWeight;
    }

    // Update is called once per frame
    void Update()
    {
        Rig.weight = Mathf.Lerp(Rig.weight, TargetWeight, Time.deltaTime * Soothing);
    }

    public void SetTargetWeight(float val)
    {
        TargetWeight = val;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateAimState();
    }

    public void SetAimTargetPosition(Vector3 pos)
    {
        TargetTransform.position = pos;
        PreviousAimLocation = TargetTransform.position;
    }

    public Vector3 GetAimTargetPosition()
    {
        return PreviousAimLocation;
    }

    public void LookAtAimTarget()
    {
        Vector3 dir = GetAimDirection(PlayerCharactersRef.GetPointOfContact());
        PlayerCharactersRef.LookAtDirection(dir);
    }

    public Vector3 GetAimDirection(Vector3 EmitterPosition)
    {
        Vector3 dir = GetAimTargetPosition() - EmitterPosition;
        dir.Normalize();
        return dir;
    }

    private void UpdateAimState()
    {
        if (PlayerCharactersRef == null)
            return;

        PlayerMovementState PMS = PlayerCharactersRef.GetPlayerManager().GetPlayerState().GetPlayerMovementState();

        if (PMS is not PlayerAimState)
        {
            SetTargetWeight(0f);
            return;
        }

        LookAtAimTarget();
        SetTargetWeight(1f);
    }
}
