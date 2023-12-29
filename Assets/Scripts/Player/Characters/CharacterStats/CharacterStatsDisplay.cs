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
    [SerializeField] TextMeshProUGUI characterStatsNameText, characterStatsValueText;
    [SerializeField] bool ShowOnlyBaseStats;
    private CharacterData characterData;

    public void SetCharacterData(CharacterData c)
    {
        characterData = c;
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
                characterStatsValueText.text = ShowTotalStatsCharactersValue().ToString();
        }

    }

    private float ShowTotalStatsCharactersValue()
    {
        switch (characterStatsType)
        {
            case CharacterStatsType.HP:
                return characterData.GetMaxHealth();
            case CharacterStatsType.EM:
                return characterData.GetEM();
            case CharacterStatsType.DEF:
                return characterData.GetDEF();
            case CharacterStatsType.ATK:
                return characterData.GetATK();
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
