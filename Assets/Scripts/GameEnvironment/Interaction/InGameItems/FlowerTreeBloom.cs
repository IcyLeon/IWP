using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowerTreeBloom : MonoBehaviour, IInteract
{
    private bool isInteracted;
    [SerializeField] int NoOfItemsToGive;
    [SerializeField] ScriptableObject[] PossibleItemsToAwardPlayer;
    [SerializeField] ItemTemplate ItemsSO;

    private void Start()
    {
        isInteracted = false;
    }

    public bool CanInteract()
    {
        return !isInteracted;
    }

    public Sprite GetInteractionSprite()
    {
        return ItemsSO.ItemSprite;
    }

    public void Interact()
    {
        List<Item> itemsList = new();
        ArtifactsManager AM = ArtifactsManager.GetInstance();
        int currentAmt = 0;

        do
        {
            int random = Random.Range(0, PossibleItemsToAwardPlayer.Length);
            ScriptableObject item = PossibleItemsToAwardPlayer[random];
            if (item == null)
                continue;

            switch (item)
            {
                case ItemTemplate itemTemplate:
                    Type ItemType = itemTemplate.GetTypeREF();
                    object instance = Activator.CreateInstance(ItemType, true, itemTemplate);
                    Item itemREF = (Item)instance;
                    itemsList.Add(itemREF);
                    break;
                case ArtifactsListInfo artifactsListInfo:
                    int randomArtifactsInfo = Random.Range(0, artifactsListInfo.artifactsInfo.Length);
                    ArtifactsInfo artifactsInfo = artifactsListInfo.artifactsInfo[randomArtifactsInfo];
                    if (artifactsInfo != null)
                    {
                        int randomArtifactsSO = Random.Range(0, artifactsInfo.artifactSOList.Length);
                        ArtifactsSO artifactsSO = artifactsInfo.artifactSOList[randomArtifactsSO];

                        Rarity randomRarity = Rarity.ThreeStar;
                        if (AssetManager.isInProbabilityRange(0.3f))
                        {
                            randomRarity = Rarity.FiveStar;
                        }
                        else
                        {
                            if (AssetManager.isInProbabilityRange(0.6f))
                            {
                                randomRarity = Rarity.ThreeStar;
                            }
                            else
                            {
                                randomRarity = Rarity.FourStar;
                            }
                        }
                        Artifacts artifact = AM.CreateArtifact(artifactsSO.artifactType, artifactsInfo.ArtifactSet, randomRarity);
                        itemsList.Add(artifact);
                    }    

                    break;

                default:
                    break;
            }
            currentAmt++;

        } while (currentAmt < NoOfItemsToGive);

        AssetManager.GetInstance().SpawnObtainedUI();
        InventoryManager.OnCallGivePlayerItems(itemsList);
        isInteracted = true;
    }

    public string InteractMessage()
    {
        return ItemsSO.ItemName;
    }

    public void OnInteractExit(IInteract interactComponent)
    {

    }

    public void OnInteractUpdate(IInteract interactComponent)
    {

    }
}
