using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterIconButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image CharacterIconImage;
    public delegate void onCharacterButtonClick(CharacterIconButton c);
    public onCharacterButtonClick OnCharacterButtonClick;
    private CharacterData characterData;

    public CharacterData GetCharacterData()
    {
        return characterData;
    }
    public void SetCharacterData(CharacterData c)
    {
        characterData = c;

        if (characterData != null)
        {
            CharacterIconImage.sprite = characterData.GetPlayerCharacterSO().PartyIcon;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCharacterButtonClick?.Invoke(this);
    }
}
