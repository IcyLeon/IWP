using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HeadText;
    [SerializeField] Button ConfirmButton;
    [SerializeField] Button CancelButton;

    protected virtual void Start()
    {
        MainUI.GetInstance().SetPaused(true);
        CancelButton.onClick.AddListener(Close);
        ConfirmButton.onClick.AddListener(ConfirmEvent);
    }
    protected virtual void ConfirmEvent()
    {
    }
    protected virtual void Close()
    {
        MainUI.GetInstance().SetPaused(false);
        Destroy(gameObject);
    }

    public void SetMessage(string headtext)
    {
        HeadText.text = headtext;
    }
}
