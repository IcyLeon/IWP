using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ArtifactStrongBoxes : ConsumableItem
{
    private ArtifactStrongBoxesSO artifactStrongBoxesSO;

    private void GetRandomArtifacts()
    {
        if (artifactStrongBoxesSO == null)
            return;

        ArtifactsSO[] artifactsSO = artifactStrongBoxesSO.ArtifactsInfo.artifactSOList;
        if (artifactsSO.Length == 0) {
            Debug.Log("No Artifacts set in this scriptable object");
            return;
        }

        ArtifactsSO ArtifactsSO = artifactsSO[Random.Range(0, artifactsSO.Length)];
        ArtifactsManager.GetInstance().AddArtifactsToInventory(ArtifactsSO.artifactType, artifactStrongBoxesSO.ArtifactsInfo.ArtifactSet, Rarity.FiveStar);
    }

    public override void Use(int Useamount)
    {
        GetRandomArtifacts();
        base.Use(Useamount);
    }

    public ArtifactStrongBoxes(bool isNew, ItemTemplate itemSO) : base(isNew,itemSO)
    {
        artifactStrongBoxesSO = GetItemSO() as ArtifactStrongBoxesSO;
    }

}
