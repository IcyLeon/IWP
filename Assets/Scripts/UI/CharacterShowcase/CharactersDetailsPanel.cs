using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharactersDetailsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CharacterNameText;
    [SerializeField] TextMeshProUGUI CharacterLevelText;
    [SerializeField] TextMeshProUGUI CharacterMaxLevelText;
    [SerializeField] TextMeshProUGUI CharacterDescText;
    [SerializeField] TextMeshProUGUI CharacterEXP;
    [SerializeField] Slider ExpSlider;
    [SerializeField] UpgradeCharacterSO UpgradeCharacterSO;
    [SerializeField] GameObject CharacterStatsDisplayContent;
    private CharacterStatsDisplay[] CharacterStatsDisplayList;
    private CharacterData characterData;

    public void Start()
    {
        ExpSlider.value = 0;
    }
    public void Init(CharacterData c)
    {
        characterData = c;
        CharacterStatsDisplayList = CharacterStatsDisplayContent.GetComponentsInChildren<CharacterStatsDisplay>(true);
        if (characterData != null)
        {
            foreach (var CharacterStatsDisplay in CharacterStatsDisplayList)
            {
                CharacterStatsDisplay.SetCharacterData(characterData);
            }

            CharacterNameText.text = characterData.GetPlayerCharacterSO().ItemName;
            CharacterDescText.text = characterData.GetPlayerCharacterSO().ItemDesc;
            UpdateLevel();
        }
    }

    // Update is called once per frame
    void UpdateLevel()
    {
        if (characterData == null)
            return;

        ExpSlider.value = characterData.GetExpAmount() / UpgradeCharacterSO.CharacterUpgradeCost[characterData.GetLevel() - 1];
        CharacterEXP.text = characterData.GetExpAmount() + "/" + UpgradeCharacterSO.CharacterUpgradeCost[characterData.GetLevel() - 1];
        CharacterEXP.gameObject.SetActive(characterData.GetMaxLevel() != characterData.GetLevel());
        CharacterLevelText.text = characterData.GetLevel().ToString();
        CharacterMaxLevelText.text = characterData.GetMaxLevel().ToString();

        foreach(var CharacterStatsDisplay in CharacterStatsDisplayList)
        {
            CharacterStatsDisplay.DisplayCharactersStat();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLevel();
    }
}
