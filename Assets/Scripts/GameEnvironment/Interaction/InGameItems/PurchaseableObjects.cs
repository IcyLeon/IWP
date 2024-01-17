using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PurchaseableObjects : MonoBehaviour, IGamePurchase
{
    [SerializeField] CurrencyType PurchaseableType;
    private WorldText worldText;
    [SerializeField] protected Transform WorldTextPivotTransform;
    [SerializeField] protected PurchaseableObjectSO PurchaseableObjectSO;
    private AssetManager assetManager;
    protected PlayerManager PM;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        assetManager = AssetManager.GetInstance();
        worldText = assetManager.SpawnWorldText(assetManager.GetCurrencySprite(PurchaseableType), GetCost().ToString(), WorldTextPivotTransform);
        worldText.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
    }

    public virtual bool CanInteract()
    {
        return true;
    }

    public Sprite GetInteractionSprite()
    {
        if (GetPurchaseableObjectSO() == null)
            return null;

        return GetPurchaseableObjectSO().PurchaseableObjectSprite;
    }

    protected void SetOwner(PlayerManager PM)
    {
        this.PM = PM;
    }

    protected PlayerManager GetOwner()
    {
        return PM;
    }
    public virtual int GetCost()
    {
        if (GetPurchaseableObjectSO() == null)
            return 0;

        return GetPurchaseableObjectSO().BasePrice;
    }

    public PurchaseableObjectSO GetPurchaseableObjectSO()
    {
        return PurchaseableObjectSO;
    }

    protected virtual void PurchaseAction(PlayerManager PM)
    {
    }

    public virtual void Interact(PlayerManager PM)
    {
        if (isPurchaseable())
        {
            PurchaseAction(PM);
            InventoryManager.GetInstance().RemoveCurrency(PurchaseableType, GetCost());
        }
        else
            assetManager.OpenMessageNotification("Insufficient " + AssetManager.CurrencyText(PurchaseableType) + "!");
    }

    private bool isPurchaseable()
    {
        return (GetCost() <= InventoryManager.GetInstance().GetCurrency(PurchaseableType));
    } 

    public string InteractMessage()
    {
        return GetPurchaseableObjectSO().PurchaseableObjectName;
    }

    public virtual void OnInteractUpdate(IInteract interactComponent)
    {
        worldText.gameObject.SetActive(true);
    }
    public virtual void OnInteractExit(IInteract interactComponent)
    {
        worldText.gameObject.SetActive(false);
    }
}
