using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneEnum
{
    GAME,
    SHOP
}

public class SceneManager : MonoBehaviour
{
    private static SceneManager instance;
    [SerializeField] Image ProgressBar;
    [SerializeField] GameObject LoadingCanvas;
    public event Action OnSceneChanged;
    private float TargetProgress = 0f;

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
        //StartCoroutine(LoadSceneAsync(GetSceneName(scene)));
        LoadSceneAsync(GetSceneName(scene));
    }

    private async void LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        LoadingCanvas.SetActive(true);
        ProgressBar.fillAmount = TargetProgress = 0f;

        do
        {
            await Task.Delay(500);
            TargetProgress = asyncOperation.progress;
        } while (asyncOperation.progress < 0.9f);

        await Task.Delay(2500);
        TargetProgress = asyncOperation.progress;
        asyncOperation.allowSceneActivation = true;
        await Task.Delay(1000);

        LoadingCanvas.SetActive(false);

        OnSceneChanged?.Invoke();
    }

    private void Update()
    {
        ProgressBar.fillAmount = Mathf.MoveTowards(ProgressBar.fillAmount, TargetProgress, Time.deltaTime * 2f);
    }

    //private IEnumerator LoadSceneAsync(string sceneName)
    //{
    //    AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    //    //asyncOperation.allowSceneActivation = false;
    //    LoadingCanvas.SetActive(true);
    //    ProgressBar.fillAmount = 0f;

    //    while (!asyncOperation.isDone)
    //    {
    //        ProgressBar.fillAmount = asyncOperation.progress;
    //        yield return null;
    //    }

    //    ProgressBar.fillAmount = asyncOperation.progress;
    //    //asyncOperation.allowSceneActivation = true;
    //    LoadingCanvas.SetActive(false);

    //    OnSceneChanged?.Invoke();
    //}

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
