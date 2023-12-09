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

    private void Awake()
    {
        canBuy = true;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab, transform).GetComponent<HealthBarScript>();
        healthBarScript.transform.localPosition = GetWorldTxtLocalPosition();
        healthBarScript.Init(false, true);
    }

    public void SetKillerData(FriendlyKillerData f){
        friendlyKillerData = f;
        if (GetFriendlyKillerData() != null)
        {
            PurchaseAction();
        }
    }

    public FriendlyKillerData GetFriendlyKillerData()
    {
        return friendlyKillerData;
    }
    protected float GetDetectionRange()
    {
        if (GetFriendlyKillerData() == null)
            return 0;

        return GetFriendlyKillerData().GetDetectionRange();
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
            }
            if (GetFriendlyKillerData() == null)
            {
                friendlyKillerData = new FriendlyKillerData(GetFriendlyKillerSO());
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
            if (friendlyKillerData != null)
            {
                healthBarScript.SetupMinAndMax(0, friendlyKillerData.GetMaxHealth());
                healthBarScript.UpdateHealth(friendlyKillerData.GetCurrentHealth());
            }
            healthBarScript.gameObject.SetActive(!canBuy && !IsDead());
        }
    }

    private void UpdateDead()
    {
        if (IsDead())
        {
            FriendlyKillerHandler.GetInstance().RemoveKillerToList(friendlyKillerData);
            friendlyKillerData = null;
            if (animator != null)
            {
                if (Characters.ContainsParam(animator, "Deactivated"))
                {
                    animator.SetTrigger("Deactivated");
                }
            }
            GameObject go = Instantiate(AssetManager.GetInstance().FirePrefab, transform.position, Quaternion.identity);
        }
    }

    public virtual bool IsDead()
    {
        if (GetFriendlyKillerData() == null)
            return true;

        return GetFriendlyKillerData().GetCurrentHealth() <= 0 && !canBuy;
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

        GetFriendlyKillerData().SetCurrentHealth(GetFriendlyKillerData().GetCurrentHealth() - damageAmt);
        AssetManager.GetInstance().SpawnWorldText_Elemental(position, elemental, damageAmt.ToString());
        UpdateDead();

        return e;
    }

    public virtual Vector3 GetPointOfContact()
    {
        return transform.position;
    }
}
