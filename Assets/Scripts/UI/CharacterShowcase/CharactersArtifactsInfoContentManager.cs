using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersArtifactsInfoContentManager : MonoBehaviour
{
    private CharactersShowcaseManager CharactersShowcaseManager;
    private CharacterData characterData;
    [SerializeField] RectTransform ArtifactPivotPoint;
    [SerializeField] CurrentCharacterArtifacts[] CurrentCharacterArtifactsList;
    [SerializeField] Transform CharacterArtifactStatsDisplayContent;

    public CharactersShowcaseManager GetCharactersShowcaseManager()
    {
        return CharactersShowcaseManager;
    }

    private void RotateAroundPivot()
    {
        ArtifactPivotPoint.rotation *= Quaternion.Euler(0, 0, Time.unscaledDeltaTime * 5f);

        if (ArtifactPivotPoint.rotation.eulerAngles.z >= 360f)
        {
            // Subtract 360 degrees to keep the rotation within the valid range
            ArtifactPivotPoint.rotation = Quaternion.Euler(0, 0, ArtifactPivotPoint.rotation.eulerAngles.z - 360f);
        }

        for (int i = 0; i < CurrentCharacterArtifactsList.Length; i++)
        {
            CurrentCharacterArtifacts artifactBubble = CurrentCharacterArtifactsList[i];
            artifactBubble.transform.rotation = Quaternion.identity;
        }
    }

    public void Init(CharactersShowcaseManager CharactersShowcaseManager, CharacterData c)
    {
        this.CharactersShowcaseManager = CharactersShowcaseManager;
        characterData = c;

        for (int i = 0; i < CurrentCharacterArtifactsList.Length; i++)
        {
            var artifactBubble = CurrentCharacterArtifactsList[i];
            InitPosition(artifactBubble, i);
            artifactBubble.SetCharacterData(GetCharacterData());
            artifactBubble.UpdateCurrentArtifact();
        }

        foreach (var CharacterArtifactStats in CharacterArtifactStatsDisplayContent.GetComponentsInChildren<CharacterArtifactStatsDisplay>())
        {
            CharacterArtifactStats.SetCharacterData(characterData);
        }

    }

    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    private void InitPosition(CurrentCharacterArtifacts artifactBubble, int idx)
    {
        float length = 250f;
        Vector2 dir = Quaternion.Euler(0, 0, Mathf.Rad2Deg * ((Mathf.PI / CurrentCharacterArtifactsList.Length) * (idx * 2f))) * Vector2.right;
        artifactBubble.GetComponent<RectTransform>().anchoredPosition = dir.normalized * length;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < CurrentCharacterArtifactsList.Length; i++)
        {
            var artifactBubble = CurrentCharacterArtifactsList[i];
            artifactBubble.ArtifactsBubbleClick += SelectedArtifactsBubbleClick;
        }
    }

    private void Update()
    {
        RotateAroundPivot();
        UpdateArtifactsDisplay();
    }

    private void UpdateArtifactsDisplay()
    {
        foreach (var CharacterArtifactStats in CharacterArtifactStatsDisplayContent.GetComponentsInChildren<CharacterArtifactStatsDisplay>())
        {
            CharacterArtifactStats.DisplayCharactersArtifactsStat();
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
        MainUI.GetInstance().GetDisplayItemsStatsManager().OpenDisplayItemsStatsManager(cca.GetSelectedArtifactsBubble().GetArtifacts());
        GetCharactersShowcaseManager().transform.GetChild(0).gameObject.SetActive(false);
    }
}
