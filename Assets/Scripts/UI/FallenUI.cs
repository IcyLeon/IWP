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
        if (characterManager.GetPlayerManager() == null)
            return;
        if (characterManager.GetPlayerManager().GetAliveCharacters() != null)
            return;

        SceneManager.GetInstance().ChangeScene(SceneEnum.SHOP);
        ReviveAllCharacters();
        EnemyManager.GetInstance().ResetEverything();
        RemoveAllTurrets();
    }

    void ReviveAllCharacters()
    {
        for (int i = 0; i < characterManager.GetPlayerManager().GetCharactersOwnedList().Count; i++)
        {
            CharacterData pc = characterManager.GetPlayerManager().GetCharactersOwnedList()[i];
            characterManager.GetPlayerManager().HealCharacterBruteForce(pc, pc.GetMaxHealth());
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
