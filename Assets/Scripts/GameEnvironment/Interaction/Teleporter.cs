using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour, IInteract
{
    private EnemyManager EnemyManager;
    private SceneManager sceneManager;

    public void Start()
    {
        sceneManager = SceneManager.GetInstance();
        EnemyManager = EnemyManager.GetInstance();
    }


    [SerializeField] SceneEnum scene;

    public void Interact(PlayerManager PM)
    {
        if (scene == SceneEnum.GAME)
        {
            int CurrentWave = EnemyManager.GetCurrentWave();
            if (CurrentWave % 4 == 0 && CurrentWave != 0)
            {
                scene = SceneEnum.BOSS;
            }
        }
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
