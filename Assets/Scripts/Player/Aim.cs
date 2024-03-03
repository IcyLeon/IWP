using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] protected PlayerCharacters PlayerCharactersRef;
    protected Vector3 AimTargetPosition;
    [SerializeField] AimRigLookAt AimLookAt;

    // Update is called once per frame
    void Update()
    {
        AimLookAt.SetTargetPosition(AimTargetPosition);
        UpdateAimState();
    }

    public void SetAimTargetPosition(Vector3 pos)
    {
        AimTargetPosition = pos;
    }

    public void LookAtAimTarget()
    {
        Vector3 dir = GetAimDirection();
        dir.y = 0;
        dir.Normalize();
        PlayerCharactersRef.LookAtDirection(dir);
    }

    public Vector3 GetAimDirection()
    {
        Vector3 dir = AimTargetPosition - PlayerCharactersRef.GetPlayerManager().GetPlayerOffsetPosition().position;
        dir.Normalize();
        return dir;
    }

    private void UpdateAimState()
    {
        if (PlayerCharactersRef == null)
            return;

        PlayerMovementState PMS = PlayerCharactersRef.GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState();

        if (PMS is not PlayerAimState)
        {
            AimLookAt.SetTargetWeight(0f);
            return;
        }

        LookAtAimTarget();
        AimLookAt.SetTargetWeight(1f);
    }
}
