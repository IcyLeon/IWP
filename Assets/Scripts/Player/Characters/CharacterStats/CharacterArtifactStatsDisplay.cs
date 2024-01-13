using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterArtifactStatsDisplay : MonoBehaviour
{
    [SerializeField] Artifacts.ArtifactsStat ArtifactsStat;
    [SerializeField] TextMeshProUGUI characterStatsNameText, characterStatsValueText;
    private CharacterData characterData;

    public void SetCharacterData(CharacterData c)
    {
        characterData = c;
    }

    public void DisplayCharactersArtifactsStat()
    {
        if (characterData == null)
            return;

        if (characterStatsNameText)
            characterStatsNameText.text = CharacterStatsDisplay.GetStatsName(ArtifactsStat);

        if (characterStatsValueText)
        {
            characterStatsValueText.text = "+" + Mathf.RoundToInt(ArtifactsManager.GetTotalArtifactValueStatsIncludePercentageAndBaseStats(characterData, ArtifactsStat)).ToString();
        }

    }

}
