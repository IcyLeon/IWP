using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoContentManager : MonoBehaviour
{
    [SerializeField] CharactersDetailsPanel characterInfoDetailsPanel;
    private CharactersShowcaseManager CharactersShowcaseManager;
    private CharacterData characterData;

    public CharactersShowcaseManager GetCharactersShowcaseManager()
    {
        return CharactersShowcaseManager;
    }
    public void Init(CharactersShowcaseManager CharactersShowcaseManager, CharacterData c)
    {
        this.CharactersShowcaseManager = CharactersShowcaseManager;
        characterData = c;
        characterInfoDetailsPanel.Init(characterData);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
