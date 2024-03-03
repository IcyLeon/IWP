using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.ProBuilder;

public class AimRigLookAt : MonoBehaviour
{
    [SerializeField] Rig Rig;
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
    }

    public void SetTargetWeight(float val)
    {
        TargetWeight = val;
    }
}
