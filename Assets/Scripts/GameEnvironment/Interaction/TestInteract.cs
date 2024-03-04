using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteract : MonoBehaviour, IInteract
{
    public void Interact(PlayerManager PM)
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

    public void OnInteractEnter(IInteract interactComponent)
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
