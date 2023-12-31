using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayArtifactStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ArtifactStatNameText, ArtifactStatValueText, ArtifactIncreasePreviewValueText;
    [SerializeField] GameObject IncreasePreviewValueContent;
    private Artifacts artifacts;

    // Update is called once per frame
    public void DisplayArtifactsStat(string Name, ArtifactStatsInfo a, Artifacts artifacts)
    {
        this.artifacts = artifacts;
        string StatsValue = Mathf.Approximately(a.GetStatsValue(), Mathf.Round(a.GetStatsValue()))
        ? a.GetStatsValue().ToString("F0")
        : a.GetStatsValue().ToString("F1");

        if (ArtifactStatValueText)
        {
            ArtifactStatValueText.text = StatsValue;
            ArtifactStatNameText.text = Name;
        }
        else
            ArtifactStatNameText.text = Name + "+" + StatsValue;

        if (CheckIfInBetweenStats_PERCENT(a.GetArtifactsStat()))
        {
            if (ArtifactStatValueText)
                ArtifactStatValueText.text = StatsValue + "%";
            else
                ArtifactStatNameText.text = Name + "+" + StatsValue + "%";
        }
    }

    public void DisplayIncreaseValueMainStats(int increaselevelPreview, ArtifactStatsInfo a)
    {
        IncreasePreviewValueContent.SetActive(increaselevelPreview > 0 && artifacts != null);
        string statsValue = artifacts.GetNextMainStatsValue(increaselevelPreview).ToString();
        if (artifacts != null)
        {
            if (CheckIfInBetweenStats_PERCENT(a.GetArtifactsStat()))
                ArtifactIncreasePreviewValueText.text = statsValue + "%";
            else                                        
                ArtifactIncreasePreviewValueText.text = statsValue;
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
