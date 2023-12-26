using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharactersDetailsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CharacterNameText;
    [SerializeField] TextMeshProUGUI CharacterLevelText;
    [SerializeField] TextMeshProUGUI CharacterMaxLevelText;
    [SerializeField] TextMeshProUGUI CharacterDescText;
    private CharacterData characterData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(CharacterData c)
    {
        characterData = c;
        if (characterData != null)
        {
            CharacterNameText.text = characterData.GetPlayerCharacterSO().ItemName;
            CharacterLevelText.text = characterData.GetLevel().ToString();
            CharacterMaxLevelText.text = characterData.GetMaxLevel().ToString();
            CharacterDescText.text = characterData.GetPlayerCharacterSO().ItemDesc;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
