using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerIndicatorInteraction : MonoBehaviour, IWorldMarker
{
    [SerializeField] Transform MarkerParentPivotTransform;
    [SerializeField] Sprite Icon;

    public Transform GetMarkerParentPivotTransform()
    {
        return MarkerParentPivotTransform;
    }

    public Sprite GetWorldMarkerSprite()
    {
        return Icon;
    }

    public void SpawnMarker()
    {
        GameObject go = Instantiate(AssetManager.GetInstance().IconPrefab, GetMarkerParentPivotTransform());
        go.transform.localPosition = Vector3.zero;
        go.transform.GetChild(0).GetComponent<Image>().sprite = GetWorldMarkerSprite();
    }

    void Start()
    {
        SpawnMarker();
    }
}
