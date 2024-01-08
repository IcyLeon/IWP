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
        ArtifactsManager AM = ArtifactsManager.GetInstance();
        ArtifactsSO[] artifactsSO = artifactStrongBoxesSO.ArtifactsInfo.artifactSOList;

        if (artifactsSO.Length == 0) {
            Debug.Log("No Artifacts set in this scriptable object");
            return;
        }

        ArtifactsSO ArtifactsSO = artifactsSO[Random.Range(0, artifactsSO.Length)];
        Artifacts artifacts = AM.CreateArtifact(ArtifactsSO.artifactType, artifactStrongBoxesSO.ArtifactsInfo.ArtifactSet, Rarity.FiveStar);
        InventoryManager.GetInstance().AddItems(artifacts);
    }

    public override void Use(int Useamount)
    {
        for (int i = 0; i < Useamount; i++)
        {
            if (amount > 0)
            {
                GetRandomArtifacts();
                base.Use(Useamount);
            }
        }
    }

    public ArtifactStrongBoxes(bool isNew, ItemTemplate itemSO) : base(isNew,itemSO)
    {
        artifactStrongBoxesSO = GetItemSO() as ArtifactStrongBoxesSO;
    }

}
