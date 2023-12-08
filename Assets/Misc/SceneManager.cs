using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneEnum
{
    GAME,
    SHOP
}

public class SceneManager : MonoBehaviour
{
    private static SceneManager instance;
    public event Action OnSceneChanged;
    private bool isChangingScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        isChangingScene = false;
    }

    public static SceneManager GetInstance()
    {
        return instance;
    }

    public void ChangeScene(SceneEnum scene)
    {
        if (!isChangingScene)
            StartCoroutine(LoadSceneAsync(GetSceneName(scene)));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        isChangingScene = true;
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        isChangingScene = false;
        OnSceneChanged?.Invoke();
    }

    private string GetSceneName(SceneEnum SceneEnum)
    {
        switch (SceneEnum)
        {
            case SceneEnum.GAME:
                return "GameScene";
            case SceneEnum.SHOP:
                return "ShopScene";
        }
        return null;
    }
}
