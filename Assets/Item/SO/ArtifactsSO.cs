using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ArtifactsSO", menuName = "ScriptableObjects/ArtifactsSO")]
public class ArtifactsSO : ItemTemplate
{
    public ArtifactType artifactType;

    public override Type GetType()
    {
        return typeof(Artifacts);
    }

    public override Category GetCategory()
    {
        return Category.ARTIFACTS;
    }
    public override string GetItemType()
    {
        return ArtifactsManager.GetInstance().GetArtifactPieceName(artifactType);
    }
}
