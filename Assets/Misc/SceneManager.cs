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
    }

    public static SceneManager GetInstance()
    {
        return instance;
    }

    public void ChangeScene(SceneEnum scene)
    {
        StartCoroutine(LoadSceneAsync(GetSceneName(scene)));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            // You can implement loading progress or other intermediate steps here
            yield return null;
        }

        OnSceneChanged?.Invoke();
    }

    public string GetSceneName(SceneEnum t)
    {
        switch (t)
        {
            case SceneEnum.GAME:
                return "SampleScene";
            case SceneEnum.SHOP:
                return "ShopScene";
        }
        return null;
    }
}
