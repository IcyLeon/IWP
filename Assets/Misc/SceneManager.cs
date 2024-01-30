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
    SHOP,
    BOSS, 
    MAIN_MENU
}

public class SceneManager : MonoBehaviour
{
    private Dictionary<string, SceneEnum> sceneNameToEnum = new();

    private static SceneManager instance;
    [SerializeField] Image ProgressBar;
    [SerializeField] GameObject LoadingCanvas;
    public static Action<SceneEnum> OnSceneChanged = delegate { };
    private float TargetProgress = 0f;
    private SceneEnum currentScene;

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

        // Initialize the reverse mapping
        foreach (SceneEnum sceneEnum in Enum.GetValues(typeof(SceneEnum)))
        {
            string sceneName = GetSceneName(sceneEnum);
            sceneNameToEnum.Add(sceneName, sceneEnum);
        }
    }
    private void Start()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        currentScene = GetSceneEnum(currentSceneName);
        OnSceneChanged?.Invoke(GetCurrentScene());
    }

    public static SceneManager GetInstance()
    {
        return instance;
    }

    public void ChangeScene(SceneEnum scene)
    {
        LoadSceneAsync(GetSceneName(scene));
        currentScene = scene;
    }

    public SceneEnum GetCurrentScene()
    {
        return currentScene;
    }

    private async void LoadSceneAsync(string sceneName)
    {
        MainUI mainUI = MainUI.GetInstance();
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        LoadingCanvas.SetActive(true);
        ProgressBar.fillAmount = TargetProgress = 0f;

        if (mainUI)
            mainUI.SetPaused(true);
        do
        {
            await Task.Delay(500);
            TargetProgress = asyncOperation.progress;
        } while (asyncOperation.progress < 0.9f);

        await Task.Delay(2500);
        TargetProgress = asyncOperation.progress;
        asyncOperation.allowSceneActivation = true;
        await Task.Delay(100);

        if (mainUI)
            mainUI.SetPaused(false);
        LoadingCanvas.SetActive(false);
        OnSceneChanged?.Invoke(GetCurrentScene());
    }

    private void Update()
    {
        ProgressBar.fillAmount = Mathf.MoveTowards(ProgressBar.fillAmount, TargetProgress, Time.unscaledDeltaTime * 2f);
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
            case SceneEnum.MAIN_MENU:
                return "MainMenuScene";
            case SceneEnum.GAME:
                return "GameScene";
            case SceneEnum.SHOP:
                return "ShopScene";
            case SceneEnum.BOSS:
                return "BossScene";
        }
        return null;
    }

    private SceneEnum GetSceneEnum(string sceneName)
    {
        if (sceneNameToEnum.ContainsKey(sceneName))
        {
            return sceneNameToEnum[sceneName];
        }

        Debug.LogError("SceneEnum not found for scene name: " + sceneName);
        return SceneEnum.GAME;
    }
}
