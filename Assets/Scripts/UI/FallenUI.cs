using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallenUI : MonoBehaviour
{
    [SerializeField] Button ReviveBtn;
    public static Action OnReviveChange;

    // Start is called before the first frame update
    void Start() {
        ReviveBtn.onClick.AddListener(ReviveAllCharactersEvent);
    }

    void ReviveAllCharactersEvent()
    {
        SceneManager.GetInstance().ChangeScene(SceneEnum.SHOP);
        OnReviveChange?.Invoke();
    }
}
