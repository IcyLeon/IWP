using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedArtifactsBubble : MonoBehaviour
{
    [SerializeField] Image ArtifactsBubbleImage;
    [SerializeField] Image ArtifactsItemImage;
    [SerializeField] Image PlaceholderArtifactsImage;
    private ArtifactsSO ArtifactsSO;
    private Artifacts artifacts;

    // Start is called before the first frame update
    public void UpdateSelectedItem(Artifacts artifacts, ArtifactsSO aSO)
    {
        this.artifacts = artifacts;
        ArtifactsSO = aSO;

        if (ArtifactsItemImage)
        {
            if (ArtifactsSO)
                ArtifactsItemImage.sprite = ArtifactsSO.ItemSprite;
        }

    }

    private void Update()
    {
        if (PlaceholderArtifactsImage)
        {
            PlaceholderArtifactsImage.gameObject.SetActive(ArtifactsSO == null);
        }
        if (ArtifactsItemImage)
        {
            ArtifactsItemImage.gameObject.SetActive(ArtifactsSO != null);
        }
    }

    public void UpdatePlaceholderArtifactsItemSprite(Sprite sprite)
    {
        PlaceholderArtifactsImage.sprite = sprite;
    }

    public Artifacts GetArtifacts()
    {
        return artifacts;
    }
}
