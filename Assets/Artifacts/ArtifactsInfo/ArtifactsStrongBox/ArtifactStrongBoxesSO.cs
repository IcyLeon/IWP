using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ArtifactStrongBoxesSO")]
public class ArtifactStrongBoxesSO : ItemTemplate
{
    public ArtifactsInfo ArtifactsInfo;

    public override Type GetType()
    {
        return typeof(ArtifactStrongBoxes);
    }

    public override Category GetCategory()
    {
        return Category.COLLECTIBLES;
    }
    public override string GetItemType()
    {
        return "Strong Box";
    }
}
