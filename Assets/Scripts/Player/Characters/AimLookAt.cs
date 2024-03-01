using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.ProBuilder;

public class AimLookAt : MonoBehaviour
{
    [SerializeField] Rig Rig;
    [SerializeField] PlayerCharacters PlayerCharactersRef;
    [SerializeField] Transform TargetTransform;
    private float Soothing = 10f;
    private float TargetWeight;

    void Awake()
    {
        SetTargetWeight(0f);
        Rig.weight = TargetWeight;
    }

    public void SetTargetPosition(Vector3 Position)
    {
        TargetTransform.position = Position;
    }

    // Update is called once per frame
    void Update()
    {
        Rig.weight = Mathf.Lerp(Rig.weight, TargetWeight, Time.deltaTime * Soothing);
        UpdateAimState();
    }

    void UpdateAimState()
    {
        PlayerMovementState PMS = PlayerCharactersRef.GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState();

        if (PMS is not PlayerAimState)
        {
            SetTargetWeight(0f);
            return;
        }
        SetTargetWeight(1f);
    }

    public void SetTargetWeight(float val)
    {
        TargetWeight = val;
    }
}
