using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CurrentCharacterArtifacts;

public class CharactersArtifactsInfoContentManager : MonoBehaviour
{
    private CharactersShowcaseManager CharactersShowcaseManager;
    private CharacterData characterData;
    [SerializeField] CurrentCharacterArtifacts[] CurrentCharacterArtifactsList;
    public CharactersShowcaseManager GetCharactersShowcaseManager()
    {
        return CharactersShowcaseManager;
    }

    public void Init(CharactersShowcaseManager CharactersShowcaseManager, CharacterData c)
    {
        this.CharactersShowcaseManager = CharactersShowcaseManager;
        characterData = c;

        foreach (var artifactBubble in CurrentCharacterArtifactsList)
        {
            artifactBubble.SetCharacterData(GetCharacterData());
            artifactBubble.UpdateCurrentArtifact();
        }
    }

    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(var artifactBubble in CurrentCharacterArtifactsList)
        {
            artifactBubble.ArtifactsBubbleClick += SelectedArtifactsBubbleClick;
        }
    }

    private void OnDestroy()
    {
        foreach (var artifactBubble in CurrentCharacterArtifactsList)
        {
            artifactBubble.ArtifactsBubbleClick -= SelectedArtifactsBubbleClick;
        }
    }

    void SelectedArtifactsBubbleClick(CurrentCharacterArtifacts cca)
    {
        MainUI.GetInstance().GetDisplayItemsStatsManager().SetCurrentSelectedItem(cca.GetSelectedArtifactsBubble().GetArtifacts());
        GetCharactersShowcaseManager().transform.GetChild(0).gameObject.SetActive(false);
    }
}
