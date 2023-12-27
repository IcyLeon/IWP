using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurrentCharacterArtifacts : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] ArtifactType artifactType;
    [SerializeField] bool IgnoreArtifactType = true;
    private CharacterData characterData;
    [SerializeField] SelectedArtifactsBubble SelectedArtifactsBubble;
    private ArtifactsManager AM;
    public delegate void artifactsBubble(CurrentCharacterArtifacts cca);
    public artifactsBubble ArtifactsBubbleClick;

    public CharacterData GetCharacterData()
    {
        return characterData;
    }
    public void SetCharacterData(CharacterData c)
    {
        characterData = c;
    }

    private void Update()
    {
        UpdateCurrentArtifact();
    }

    public void UpdateCurrentArtifact()
    {
        if (IgnoreArtifactType)
            return;

        if (GetCharacterData() == null)
            return;

        if (AM == null)
            AM = ArtifactsManager.GetInstance();

        Artifacts artifacts = GetCharacterData().CheckIfArtifactTypeExist(artifactType);
        ArtifactsSO artifactsSO = GetArtifactsSO(artifacts);
        SelectedArtifactsBubble.UpdateSelectedItem(artifacts, artifactsSO);

        if (!artifactsSO)
        {
            SelectedArtifactsBubble.UpdatePlaceholderArtifactsItemSprite(AM.GetArtifactPiece(artifactType).artifactIconSprite);
        }
    }

    private ArtifactsSO GetArtifactsSO(Artifacts artifact)
    {
        if (artifact == null)
            return null;

        return (ArtifactsSO)artifact.GetItemSO();
    }

    public SelectedArtifactsBubble GetSelectedArtifactsBubble()
    {
        return SelectedArtifactsBubble;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IgnoreArtifactType)
            return;
        ArtifactsBubbleClick?.Invoke(this);
    }

}
