using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseableObjects : MonoBehaviour, IGamePurchase
{
    [SerializeField] protected PurchaseableObjectSO PurchaseableObjectSO;
    private bool isBought;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        isBought = false;
    }

    protected virtual void Update()
    {
    }

    public virtual bool CanInteract()
    {
        return isBought;
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


    public virtual void Interact()
    {
    }

    public virtual string InteractMessage()
    {
        return "??? Object";
    }

    public void ShowCost()
    {

    }
}
