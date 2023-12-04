using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public interface IGamePurchase : IInteract
{
    void ShowCost();
    public int GetCost();
}

public class FriendlyKillers : PurchaseableObjects, IDamage
{
    private float CurrentHealth;
    private float MaxHealth;
    protected HealthBarScript healthBarScript;
    public event Action OnDead;
    public delegate void onElementReactionHit(ElementalReactionsTrigger e);
    public onElementReactionHit OnElementReactionHit;
    private bool canBuy;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canBuy = true;
        healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab, transform).GetComponent<HealthBarScript>();
        healthBarScript.transform.localPosition = GetWorldTxtLocalPosition();
        healthBarScript.Init(false, true);
        MaxHealth = GetFriendlyKillerSO().BaseMaxHealth;
        CurrentHealth = MaxHealth;
    }

    public FriendlyKillerSO GetFriendlyKillerSO()
    {
        FriendlyKillerSO friendlyKillerSO = GetPurchaseableObjectSO() as FriendlyKillerSO;
        return friendlyKillerSO;
    }

    public override bool CanInteract()
    {
        return canBuy;
    }

    protected override void PurchaseAction()
    {
        if (canBuy)
            canBuy = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.SetupMinAndMax(0, GetMaxHealth());
            healthBarScript.UpdateHealth(GetHealth());
            healthBarScript.gameObject.SetActive(!canBuy);
        }
    }

    public virtual float GetHealth()
    {
        return CurrentHealth;
    }

    public virtual float GetMaxHealth()
    {
        return MaxHealth;
    }

    public virtual bool IsDead()
    {
        return CurrentHealth <= 0 && !canBuy;
    }

    public virtual Elements TakeDamage(Vector3 position, Elements elements, float damageAmt)
    {
        if (IsDead() || canBuy)
            return null;

        Elemental elemental;
        if (elements != null)
        {
            elemental = elements.GetElements();
        }
        else
        {
            elemental = Elemental.NONE;
        }
        Elements e = new Elements(elemental);

        SetHealth(GetHealth() - damageAmt);
        AssetManager.GetInstance().SpawnWorldText_Elemental(position, elemental, damageAmt.ToString());

        return e;
    }

    public virtual void SetHealth(float val)
    {
        CurrentHealth = val;
    }

}
