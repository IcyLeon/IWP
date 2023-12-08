using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour, IInteract
{
    private SceneManager sceneManager;

    public void Start()
    {
        sceneManager = SceneManager.GetInstance();
    }


    [SerializeField] SceneEnum scene;

    public void Interact()
    {
        sceneManager.ChangeScene(scene);
    }

    public string InteractMessage()
    {
        return "Enter Teleporter";
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
