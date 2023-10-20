using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactsSO", menuName = "ScriptableObjects/ArtifactsSO")]
public class ArtifactsSO : ItemTemplate
{
    public ArtifactType artifactType;

    public override string GetItemType()
    {
        return ArtifactsManager.instance.GetArtifactPieceName(artifactType);
    }
}
