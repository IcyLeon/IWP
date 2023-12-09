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

        SceneManager.GetInstance().ChangeScene(SceneEnum.SHOP);
        ReviveAllCharacters();
        EnemyManager.GetInstance().ResetEverything();
        RemoveAllTurrets();
    }

    void ReviveAllCharacters()
    {
        for (int i = 0; i < characterManager.GetCharactersOwnedList().Count; i++)
        {
            CharacterData pc = characterManager.GetCharactersOwnedList()[i];
            characterManager.HealCharacter(pc, pc.GetMaxHealth());
        }
    }

    void RemoveAllTurrets()
    {
        FriendlyKillerHandler f = FriendlyKillerHandler.GetInstance();
        for (int i = 0; i < f.GetFriendlyKillerDataList().Count; i++)
        {
            f.RemoveKillerToList(f.GetFriendlyKillerDataList()[i]);
        }
    }
}
