using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Artifacts;

public class DisplayArtifactStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ArtifactStatNameText, ArtifactStatValueText;


    // Update is called once per frame
    public void DisplayArtifactsStat(string Name, ArtifactsStat artifactsStat, string Value)
    {
        if (ArtifactStatValueText)
        {
            ArtifactStatValueText.text = Value;
            ArtifactStatNameText.text = Name;
        }
        else
            ArtifactStatNameText.text = Name + "+" + Value;

        if (CheckIfInBetweenStats_PERCENT(artifactsStat))
        {
            if (ArtifactStatValueText)
                ArtifactStatValueText.text = Value + "%";
            else
                ArtifactStatNameText.text = Name + "+" + Value + "%";
        }
    }

    private bool CheckIfInBetweenStats_PERCENT(ArtifactsStat stat)
    {
        for (int i = (int)ArtifactsStat.HPPERCENT; i <= (int)ArtifactsStat.CritDamage; i++)
        {
            if ((int)stat == i)
            {
                return true;
            }
        }
        return false;
    }
}
