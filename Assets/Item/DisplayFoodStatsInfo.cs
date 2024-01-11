using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayFoodStatsInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statsInfoTxt;

    public void DisplayFoodsStat(string text)
    {
        if (statsInfoTxt)
        {
            statsInfoTxt.text = text;
        }
    }
}
