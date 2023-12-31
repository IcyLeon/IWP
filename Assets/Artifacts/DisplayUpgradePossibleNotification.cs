using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayUpgradePossibleNotification : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI StatsMessageTxt;

    // Update is called once per frame
    public void UpdateNotification(string msg)
    {
        StatsMessageTxt.text = msg;
    }
}
