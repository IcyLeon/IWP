using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefaultColliderData
{
    public float Height;
    public float CenterY;
    public float Radius;
}

[System.Serializable]
public class SlopeData
{
    [field: Range(0f, 1f)] 
    public float StepHeightPercentage = 0.25f;


    [field: Range(0f, 5f)] 
    public float FloatRayDistance = 2f;

    [field: Range(0f, 50f)]
    public float StepReachForce = 25f;
}

public class ResizeableCollider : MonoBehaviour
{
    private Vector3 ColliderCenterInLocalSpace;
    private Vector3 ColliderVerticalExtents;

    [SerializeField] PlayerController pc;
    [SerializeField] DefaultColliderData DefaultColliderData;
    [SerializeField] SlopeData SlopeData;
    [SerializeField] AnimationCurve SlopeSpeedAngles;

    public AnimationCurve GetSlopeSpeedAngles()
    {
        return SlopeSpeedAngles;
    }
    private void Awake()
    {
        Resize();
    }

    private void OnValidate()
    {
        Resize();
    }

    public Vector3 GetColliderCenterInLocalSpace()
    {
        return ColliderCenterInLocalSpace;
    }

    public Vector3 GetColliderVerticalExtents()
    {
        return ColliderVerticalExtents;
    }

    public void Resize()
    {
        Initialize(gameObject);

        CalculateCapsuleColliderDimensions();
    }

    public void UpdateColliderData()
    {
        if (pc == null)
            return;

        if (pc.GetCapsuleCollider() == null)
            return;

        ColliderCenterInLocalSpace = pc.GetCapsuleCollider().center;

        ColliderVerticalExtents = new Vector3(0f, pc.GetCapsuleCollider().bounds.extents.y, 0f);
    }

    public void Initialize(GameObject gameObject)
    {
        UpdateColliderData();

        OnInitialize();
    }

    protected virtual void OnInitialize()
    {
    }

    public SlopeData GetSlopeData()
    {
        return SlopeData;
    }

    public void CalculateCapsuleColliderDimensions()
    {
        SetCapsuleColliderRadius(DefaultColliderData.Radius);

        SetCapsuleColliderHeight(DefaultColliderData.Height * (1f - GetSlopeData().StepHeightPercentage));

        RecalculateCapsuleColliderCenter();

        RecalculateColliderRadius();

        UpdateColliderData();
    }

    public void SetCapsuleColliderRadius(float radius)
    {
        if (pc.GetCapsuleCollider() == null)
            return;

        pc.GetCapsuleCollider().radius = radius;
    }

    public void SetCapsuleColliderHeight(float height)
    {
        if (pc.GetCapsuleCollider() == null)
            return;

        pc.GetCapsuleCollider().height = height;
    }

    public void RecalculateCapsuleColliderCenter()
    {
        if (pc.GetCapsuleCollider() == null)
            return;

        float colliderHeightDifference = DefaultColliderData.Height - pc.GetCapsuleCollider().height;

        Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);

        pc.GetCapsuleCollider().center = newColliderCenter;
    }

    public void RecalculateColliderRadius()
    {
        if (pc.GetCapsuleCollider() == null)
            return;

        float halfColliderHeight = pc.GetCapsuleCollider().height / 2f;

        if (halfColliderHeight >= pc.GetCapsuleCollider().radius)
        {
            return;
        }

        SetCapsuleColliderRadius(halfColliderHeight);
    }
}
