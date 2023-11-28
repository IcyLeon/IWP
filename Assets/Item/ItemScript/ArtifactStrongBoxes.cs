using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactStrongBoxes : ConsumableItem
{
    private ArtifactStrongBoxesSO artifactStrongBoxesSO;

    public override void Use(int Useamount)
    {
        base.Use(Useamount);
    }

    public ArtifactStrongBoxes(bool isNew, ItemTemplate itemSO) : base(isNew,itemSO)
    {
        artifactStrongBoxesSO = GetItemSO() as ArtifactStrongBoxesSO;
    }

}
