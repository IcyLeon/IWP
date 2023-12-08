using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteract : MonoBehaviour, IInteract
{
    public void Interact()
    {
        Debug.Log(InteractMessage());
    }

    public string InteractMessage()
    {
        return "Test";
    }

    public bool CanInteract()
    {
        return true;
    }

    public void OnInteractUpdate(IInteract interactComponent)
    {
    }
    public void OnInteractExit(IInteract interactComponent)
    {
    }

    public Sprite GetInteractionSprite()
    {
        return null;
    }
}
