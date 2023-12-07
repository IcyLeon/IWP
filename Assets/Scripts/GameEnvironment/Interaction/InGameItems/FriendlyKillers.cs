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
    [SerializeField] protected Animator animator;
    private FriendlyKillerData friendlyKillerData;
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

        friendlyKillerData = new FriendlyKillerData(GetFriendlyKillerSO());
    }

    protected float GetDetectionRange()
    {
        if (friendlyKillerData == null)
            return 0;

        return friendlyKillerData.GetDetectionRange();
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
        {
            if (animator != null)
            {
                if (Characters.ContainsParam(animator, "Activated"))
                {
                    animator.SetTrigger("Activated");
                }
                FriendlyKillerHandler.GetInstance().AddKillerToList(friendlyKillerData);
            }
            canBuy = false;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.SetupMinAndMax(0, friendlyKillerData.GetMaxHealth());
            healthBarScript.UpdateHealth(friendlyKillerData.GetCurrentHealth());
            healthBarScript.gameObject.SetActive(!canBuy);
        }
    }


    public virtual bool IsDead()
    {
        return friendlyKillerData.GetCurrentHealth() <= 0 && !canBuy;
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

        friendlyKillerData.SetCurrentHealth(friendlyKillerData.GetCurrentHealth() - damageAmt);
        AssetManager.GetInstance().SpawnWorldText_Elemental(position, elemental, damageAmt.ToString());

        return e;
    }

}
