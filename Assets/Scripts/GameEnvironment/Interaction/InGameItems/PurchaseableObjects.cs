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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        assetManager = AssetManager.GetInstance();
        worldText = assetManager.SpawnWorldText(GetCost().ToString(), transform);
        worldText.transform.localPosition = new Vector3(0, WorldTextPivotTransform.transform.localPosition.y, 0);
        worldText.gameObject.SetActive(false);
    }

    protected Vector3 GetWorldTxtLocalPosition()
    {
        return worldText.transform.localPosition;
    }
    protected virtual void Update()
    {
    }

    public virtual bool CanInteract()
    {
        return true;
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

    protected virtual void PurchaseAction()
    {

    }

    public void Interact()
    {
        if (isPurchaseable())
            PurchaseAction();
        else
            assetManager.OpenMessageNotification("Insufficient " + AssetManager.CurrencyText(PurchaseableType) + "!");
    }

    private bool isPurchaseable()
    {
        int Cost = GetCost();
        return (Cost <= InventoryManager.GetInstance().GetCurrency(PurchaseableType));
    } 

    public virtual string InteractMessage()
    {
        return "??? Object";
    }

    public void ShowCost()
    {

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