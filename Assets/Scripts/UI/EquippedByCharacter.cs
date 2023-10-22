using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedByCharacter : MonoBehaviour
{
    [SerializeField] Image IconImage;
    [SerializeField] TextMeshProUGUI CharacterEquipTxt;
    public void UpdateContent(Sprite sprite, string characterName = "")
    {
        if (IconImage)
            IconImage.sprite = sprite;
        if (CharacterEquipTxt)
            CharacterEquipTxt.text = "Equipped: " + characterName;
    }
}
