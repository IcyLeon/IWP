using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : PurchaseableObjects
{
    [SerializeField] Material ChestMaterialPrefab;
    [SerializeField] MeshRenderer meshRenderer;
    private Material ChestMaterial;
    private bool isOpen;
    private Coroutine DissolvedDelay;

    private void Awake()
    {
        isOpen = false;
    }

    protected override void Start()
    {
        base.Start();
        ChestMaterial = Instantiate(ChestMaterialPrefab);
        meshRenderer.material = ChestMaterial;
    }

    public override bool CanInteract()
    {
        return !isOpen;
    }

    public ChestSO GetChestSO()
    {
        if (GetPurchaseableObjectSO() == null)
            return null;

        return GetPurchaseableObjectSO() as ChestSO;
    }

    protected override void PurchaseAction(PlayerManager PM)
    {
        base.PurchaseAction(PM);

        if (!isOpen)
        {
            ChestDropRandomItem(transform.position);
            isOpen = true;
        }
    }

    private void ChestDropRandomItem(Vector3 SpawnPosition)
    {
        List<Item> itemsList = new();
        int amtToDrop = Random.Range(0, GetChestSO().MaxTotalDrops + 1);
        int currentAmt = 0;
        ArtifactsManager AM = ArtifactsManager.GetInstance();

        do
        {
            int random = Random.Range(0, GetChestSO().ItemDropsList.Length);
            ScriptableObject item = GetChestSO().ItemDropsList[random];
            if (item == null)
                continue;

            switch (item)
            {
                case ItemTemplate itemTemplate:
                    Item itemREF = InventoryManager.CreateItem(itemTemplate);
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
                        if (AssetManager.isInProbabilityRange(0.1f))
                        {
                            randomRarity = Rarity.FiveStar;
                        }
                        else
                        {
                            if (AssetManager.isInProbabilityRange(0.7f))
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

        } while (currentAmt < amtToDrop);

        AssetManager.GetInstance().SpawnItemDropListGO(itemsList, SpawnPosition);

        if (DissolvedDelay == null)
            DissolvedDelay = StartCoroutine(DissolvedDelayCoroutine(1f));

        if (PM != null)
        {
            PM.GetInventoryManager().AddCurrency(CurrencyType.COINS, GetChestSO().BaseCoinsDrops + Mathf.RoundToInt(GetChestSO().BaseCoinsDrops * (EnemyManager.GetCurrentWave() - 1) * 0.287f));
            PM.GetInventoryManager().AddCurrency(CurrencyType.CASH, GetChestSO().BaseCashsDrops + Mathf.RoundToInt(GetChestSO().BaseCashsDrops * (EnemyManager.GetCurrentWave() - 1) * 0.373f));
        }
    }

    private IEnumerator DissolvedDelayCoroutine(float timer)
    {
        yield return new WaitForSeconds(timer);
        StartCoroutine(AssetManager.Dissolved(transform.root.gameObject, ChestMaterial, 5f));
    }

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        SetOwner(playerManager);
    }
    public override int GetCost()
    {
        return base.GetCost() + Mathf.RoundToInt(base.GetCost() * (EnemyManager.GetCurrentWave() - 1) * 0.478f);
    }

}
