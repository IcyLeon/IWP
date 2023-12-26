using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurrentCharacterArtifacts : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DisplayItemsStatsManager DisplayItemsStatsManager;
    [SerializeField] ArtifactType artifactType;
    [SerializeField] CharactersArtifactsInfoContentManager CharactersArtifactsInfoContentManager;
    private SelectedArtifactsBubble SelectedArtifactsBubble;
    private ArtifactsManager AM;
    void Start()
    {
        SelectedArtifactsBubble = GetComponent<SelectedArtifactsBubble>();
        AM = ArtifactsManager.GetInstance();
    }

    private void Update()
    {
        if (CharactersArtifactsInfoContentManager.GetCharacterData() != null)
        {
            Artifacts artifacts = CharactersArtifactsInfoContentManager.GetCharacterData().CheckIfArtifactTypeExist(artifactType);
            ArtifactsSO artifactsSO = GetArtifactsSO(artifacts);
            SelectedArtifactsBubble.UpdateSelectedItem(artifacts, artifactsSO);

            if (!artifactsSO)
            {
                SelectedArtifactsBubble.UpdatePlaceholderArtifactsItemSprite(AM.GetArtifactPiece(artifactType).artifactIconSprite);
            }
        }
    }

    private ArtifactsSO GetArtifactsSO(Artifacts artifact)
    {
        if (artifact == null)
            return null;

        return (ArtifactsSO)artifact.GetItemSO();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DisplayItemsStatsManager.SetCurrentSelectedItem(SelectedArtifactsBubble.GetArtifacts());
        CharactersArtifactsInfoContentManager.GetCharactersShowcaseManager().transform.GetChild(0).gameObject.SetActive(false);
    }

}
