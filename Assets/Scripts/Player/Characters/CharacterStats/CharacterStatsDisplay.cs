using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CharacterStatsType
{
    HP,
    EM,
    DEF,
    ATK,
    CRIT_DMG,
    CRIT_RATE
}

public class CharacterStatsDisplay : MonoBehaviour
{
    [SerializeField] CharacterStatsType characterStatsType;
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
            characterStatsNameText.text = GetCharactersName();

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
            case CharacterStatsType.HP:
                return characterData.GetMaxHealth(level);
            case CharacterStatsType.EM:
                return characterData.GetEM(level);
            case CharacterStatsType.DEF:
                return characterData.GetDEF(level);
            case CharacterStatsType.ATK:
                return characterData.GetATK(level);
        }
        return 0f;
    }

    private float ShowbaseStatsCharactersValue()
    {
        switch (characterStatsType)
        {
            case CharacterStatsType.HP:
                return characterData.GetPlayerCharacterSO().GetAscensionInfo(characterData.GetCurrentAscension()).BaseHP;
            case CharacterStatsType.EM:
                return 0;
            case CharacterStatsType.DEF:
                return characterData.GetPlayerCharacterSO().GetAscensionInfo(characterData.GetCurrentAscension()).BaseDEF;
            case CharacterStatsType.ATK:           
                return characterData.GetPlayerCharacterSO().GetAscensionInfo(characterData.GetCurrentAscension()).BaseATK;
        }
        return 0f;
    }


    private string GetCharactersName()
    {
        switch(characterStatsType)
        {
            case CharacterStatsType.HP:
                return "Max HP";
            case CharacterStatsType.EM:
                return "Elemental Mastery";
            case CharacterStatsType.DEF:
                return "DEF";
            case CharacterStatsType.ATK:
                return "ATK";
        }
        return null;
    } 
}
