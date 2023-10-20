using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : PopupPanel
{
    [SerializeField] TextMeshProUGUI HeadText;
    [SerializeField] Button ConfirmButton;
    [SerializeField] Button CancelButton;

    public void SetMessage(string headtext, string bodytext)
    {
        HeadText.text = headtext;
        base.SetMessage(bodytext);
    }
}
