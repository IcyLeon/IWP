using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterStatsDisplay : MonoBehaviour
{
    [SerializeField] Artifacts.ArtifactsStat characterStatsType;
    [SerializeField] TextMeshProUGUI characterStatsNameText, characterStatsValueText, characterIncreasePreviewValueText;
    [SerializeField] GameObject IncreasePreviewValueContent;
    [SerializeField] bool ShowOnlyBaseStats;
    private CharacterData characterData;

    public void SetCharacterData(CharacterData c)
    {
        characterData = c;
    }

    public void DisplayIncreaseValueStats(int increaselevelPreview)
    {
        IncreasePreviewValueContent.SetActive(increaselevelPreview > 0);
        characterIncreasePreviewValueText.text = ShowTotalStatsCharactersValue(characterData.GetLevel() + increaselevelPreview).ToString();
    }

    public void DisplayCharactersStat()
    {
        if (characterData == null)
            return;

        if (characterStatsNameText)
            characterStatsNameText.text = GetStatsName(characterStatsType);

        if (characterStatsValueText)
        {
            if (ShowOnlyBaseStats)
                characterStatsValueText.text = ShowbaseStatsCharactersValue().ToString();
            else
                characterStatsValueText.text = ShowTotalStatsCharactersValue(characterData.GetLevel()).ToString();
        }

    }

    private float ShowTotalStatsCharactersValue(int level)
    {
        switch (characterStatsType)
        {
            case Artifacts.ArtifactsStat.HP:
                return characterData.GetMaxHealth(level);
            case Artifacts.ArtifactsStat.EM:
                return characterData.GetEM(level);
            case Artifacts.ArtifactsStat.DEF:
                return characterData.GetDEF(level);
            case Artifacts.ArtifactsStat.ATK:
                return characterData.GetATK(level);
        }
        return 0f;
    }

    private float ShowbaseStatsCharactersValue()
    {
        switch (characterStatsType)
        {
            case Artifacts.ArtifactsStat.HP:
                return characterData.GetPlayerCharacterSO().GetAscensionInfo(characterData.GetCurrentAscension()).BaseHP;
            case Artifacts.ArtifactsStat.EM:
                return 0;
            case Artifacts.ArtifactsStat.DEF:
                return characterData.GetPlayerCharacterSO().GetAscensionInfo(characterData.GetCurrentAscension()).BaseDEF;
            case Artifacts.ArtifactsStat.ATK:           
                return characterData.GetPlayerCharacterSO().GetAscensionInfo(characterData.GetCurrentAscension()).BaseATK;
        }
        return 0f;
    }


    public static string GetStatsName(Artifacts.ArtifactsStat artifactType)
    {
        switch(artifactType)
        {
            case Artifacts.ArtifactsStat.HP:
                return "HP";
            case Artifacts.ArtifactsStat.EM:
                return "Elemental Mastery";
            case Artifacts.ArtifactsStat.DEF:
                return "DEF";
            case Artifacts.ArtifactsStat.ATK:
                return "ATK";
            case Artifacts.ArtifactsStat.ATKPERCENT:
                return "ATK";
            case Artifacts.ArtifactsStat.CritRate:
                return "CRIT Rate";
            case Artifacts.ArtifactsStat.CritDamage:
                return "CRIT DMG";
            case Artifacts.ArtifactsStat.DEFPERCENT:
                return "DEF";
            case Artifacts.ArtifactsStat.ER:
                return "Energy Recharge";
            case Artifacts.ArtifactsStat.HPPERCENT:
                return "HP";
        }
        return null;
    } 
}
