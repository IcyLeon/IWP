using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static ArtifactsListInfo;

public class ArtifactPieceChecker : MonoBehaviour
{
    private static ArtifactPieceChecker singleton;
    public static ArtifactPieceChecker instance
    {
        get { return singleton; }
    }

    private void Awake()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // get total amount of artifacts based on type from equip list or just from inventory list
    public int GetNumberofArtifactsINV(ArtifactsSet? artifactsSet = null, bool isFromPlayersInvInstead = false, bool IncludeLockArtifact = true)
    {
        int count = 0;
        List<Item> itemlist = new List<Item>();
        itemlist.Clear();

        if (isFromPlayersInvInstead)
            itemlist = InventoryManager.GetInstance().GetINVList();
        else
        {
            for (int i = 0; i < CharacterManager.GetInstance().GetCurrentCharacter().GetEquippedArtifactsList().Count; i++)
            {
                Artifacts artifacts = CharacterManager.GetInstance().GetCurrentCharacter().GetEquippedArtifactsList()[i];
                itemlist.Add(artifacts);
            }
        }
        for (int i = 0; i < itemlist.Count; i++)
        {
            Item item = InventoryManager.GetInstance().GetINVList()[i];
            Artifacts artifact = item as Artifacts;
            if (artifact.GetArtifactsSet() == artifactsSet || artifactsSet == null)
            {
                if (!artifact.GetLockStatus() || IncludeLockArtifact)
                    count++;
            }
        }
        return count;
    }
}
