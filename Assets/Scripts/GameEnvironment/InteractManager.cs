using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IWorldMarker
{
    Sprite GetWorldMarkerSprite();
    void SpawnMarker();
    Transform GetMarkerParentPivotTransform();
}

public interface IInteract
{
    bool CanInteract();
    void Interact(PlayerManager PM);
    string InteractMessage();
    void OnInteractEnter(IInteract interactComponent);
    void OnInteractExit(IInteract interactComponent);
    Sprite GetInteractionSprite();
}

public class InteractManager : MonoBehaviour
{
    private List<IInteract> currentIInteractList;
    private float InteractRange = 3f;

    public delegate void OnInteractChange(IInteract interact);
    public static OnInteractChange OnInteractAdd;
    public static OnInteractChange OnInteractRemove;

    // Start is called before the first frame update
    void Start()
    {
        currentIInteractList = new();
    }

    private void Update()
    {
        GetAllNearestInteractObj();
    }

    private void GetAllNearestInteractObj()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, InteractRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out IInteract interact))
            {
                if (!currentIInteractList.Contains(interact))
                {
                    interact.OnInteractEnter(interact);
                    currentIInteractList.Add(interact);
                    OnInteractAdd?.Invoke(interact);
                }
            }
        }

        // Find and remove IInteract instances not present in colliders
        for (int i = currentIInteractList.Count - 1; i >= 0; i--)
        {
            IInteract existinteract = currentIInteractList[i];

            bool foundInColliders = false;

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out IInteract colliderInteract) && colliderInteract == existinteract)
                {
                    foundInColliders = true;
                    break;
                }
            }

            if (!foundInColliders)
            {
                existinteract.OnInteractExit(existinteract);
                currentIInteractList.Remove(existinteract);
                OnInteractRemove?.Invoke(existinteract);
            }
        }

    }
}
