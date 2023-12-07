using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallenUI : MonoBehaviour
{
    [SerializeField] Button ReviveBtn;
    private CharacterManager characterManager;

    // Start is called before the first frame update
    void Start() {
        characterManager = CharacterManager.GetInstance();
        ReviveBtn.onClick.AddListener(ReviveAllCharactersEvent);
    }

    void ReviveAllCharactersEvent()
    {
        if (characterManager.GetAliveCharacters() != null)
            return;

        ReviveAllCharacters();
        SceneManager.GetInstance().ChangeScene(SceneEnum.SHOP);
    }

    void ReviveAllCharacters()
    {
        for (int i = 0; i < characterManager.GetCharactersOwnedList().Count; i++)
        {
            CharacterData pc = characterManager.GetCharactersOwnedList()[i];
            characterManager.HealCharacter(pc, pc.GetMaxHealth() * 0.35f);
        }
    }
}
