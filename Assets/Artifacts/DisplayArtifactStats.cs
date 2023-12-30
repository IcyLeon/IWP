using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayArtifactStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ArtifactStatNameText, ArtifactStatValueText;


    // Update is called once per frame
    public void DisplayArtifactsStat(string Name, ArtifactStatsInfo a)
    {
        if (ArtifactStatValueText)
        {
            ArtifactStatValueText.text = a.GetStatsValue().ToString();
            ArtifactStatNameText.text = Name;
        }
        else
            ArtifactStatNameText.text = Name + "+" + a.GetStatsValue();

        if (CheckIfInBetweenStats_PERCENT(a.GetArtifactsStat()))
        {
            if (ArtifactStatValueText)
                ArtifactStatValueText.text = a.GetStatsValue() + "%";
            else
                ArtifactStatNameText.text = Name + "+" + a.GetStatsValue() + "%";
        }
    }

    private bool CheckIfInBetweenStats_PERCENT(Artifacts.ArtifactsStat stat)
    {
        for (int i = (int)Artifacts.ArtifactsStat.HPPERCENT; i < (int)Artifacts.ArtifactsStat.TOTAL_STATS; i++)
        {
            if ((int)stat == i)
            {
                return true;
            }
        }
        return false;
    }
}
