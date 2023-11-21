using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionContentUI : MonoBehaviour
{
    [SerializeField] Transform InteractOptionsPivot;
    [SerializeField] GameObject SelectionArrowPrefab;

    public Transform GetInteractOptionsPivot()
    {
        return InteractOptionsPivot;
    }

    public GameObject SpawnInteractionArrow()
    {
        GameObject go = Instantiate(SelectionArrowPrefab, transform);
        return go;
    }
}
