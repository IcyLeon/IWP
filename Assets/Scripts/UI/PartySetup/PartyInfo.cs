using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PartyInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PartyName;
    [SerializeField] Image PartyIconImage;
    [SerializeField] HealthBarScript healthBarScript;
    [SerializeField] TextMeshProUGUI KeyText;
    private CharacterData characterData;

    public void SetKeyText(string t)
    {
        KeyText.text = t;
    }
    private void Start()
    {
        if (characterData != null)
        {
            PartyName.text = characterData.GetPlayerCharacterSO().ItemName;
            PartyIconImage.sprite = characterData.GetPlayerCharacterSO().PartyIcon;
        }
    }
    // Update is called once per frame
    void Update()
    {
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        if (healthBarScript && characterData != null)
        {
            healthBarScript.SetupMinAndMax(0, characterData.GetMaxHealth(characterData.GetLevel()));
            healthBarScript.UpdateHealth(characterData.GetHealth());
        }
    }

    public void SetCharacterData(CharacterData c)
    {
        characterData = c;
    }
}
