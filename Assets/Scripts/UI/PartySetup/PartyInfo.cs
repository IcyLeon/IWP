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
    [SerializeField] Material materialInstance;
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
            PartyIconImage.material = Instantiate(materialInstance);
        }
    }
    // Update is called once per frame
    void Update()
    {
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        if (characterData != null)
        {
            if (healthBarScript)
            {
                healthBarScript.UpdateHealth(characterData.GetHealth(), 0, characterData.GetActualMaxHealth(characterData.GetLevel()));
            }

            if (characterData.IsDead())
                SetGrayScale(0);
            else
                SetGrayScale(1);
        }
    }

    private void SetGrayScale(float val)
    {
        PartyIconImage.material.SetFloat("_Scale", val);
    }
    public void SetCharacterData(CharacterData c)
    {
        characterData = c;
    }
}
