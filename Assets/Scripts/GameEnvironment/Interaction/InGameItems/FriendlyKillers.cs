using System;
using System.Collections;
using System.Collections.Generic;
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
        base.PurchaseAction();

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
                healthBarScript.UpdateHealth(GetHealth(), 0, GetMaxHealth());
            }
            healthBarScript.gameObject.SetActive(!canBuy && !IsDead());
        }
    }

    protected virtual void UpdateDead()
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
        return GetHealth() <= 0 && !canBuy;
    }

    public virtual Elements TakeDamage(Vector3 position, Elements elements, float damageAmt, IDamage source, bool callHitInfo = true)
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
        int dmg = Mathf.RoundToInt(damageAmt);

        SetHealth(GetFriendlyKillerData().GetHealth() - dmg);
        AssetManager.GetInstance().SpawnWorldText_Elemental(position, elemental, dmg.ToString());
        UpdateDead();

        return e;
    }

    public virtual Vector3 GetPointOfContact()
    {
        return transform.position;
    }

    protected Collider[] GetAllNearestIDamage(Vector3 EmittorPos, float range, int OverlapMask, int RayCastMask = Physics.DefaultRaycastLayers)
    {
        Collider[] colliders = Physics.OverlapSphere(EmittorPos, range, OverlapMask);
        List<Collider> colliderCopy = new List<Collider>(colliders);
        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            IDamage c = colliderCopy[i].GetComponent<IDamage>();
            if (c != null)
            {
                Vector3 dir = c.GetPointOfContact() - GetPointOfContact();

                if (Physics.Raycast(GetPointOfContact(), dir.normalized, out RaycastHit hit, range, RayCastMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.GetComponent<IDamage>() == null)
                    {
                        colliderCopy.Remove(colliderCopy[i]);
                    }
                    else
                    {
                        if (hit.collider.GetComponent<GetRootParent>() != null)
                            if (hit.collider.GetComponent<GetRootParent>().GetOwner().GetComponent<IDamage>() == null)
                                colliderCopy.Remove(colliderCopy[i]);
                    }
                }

                if (c.IsDead() && i < colliderCopy.Count)
                {
                    colliderCopy.RemoveAt(i);
                }

            }
            else
            {
                if (i < colliderCopy.Count)
                    colliderCopy.RemoveAt(i);
            }
        }

        return colliderCopy.ToArray();
    }


    protected Collider GetNearestIDamage(Vector3 EmittorPos, float range, int OverlapMask, int RayCastMask = Physics.DefaultRaycastLayers)
    {
        Collider[] colliders = GetAllNearestIDamage(EmittorPos, range, OverlapMask, RayCastMask);

        if (colliders.Length == 0)
            return null;

        List<Collider> colliderCopy = new List<Collider>(colliders);

        Collider nearestCollider = colliderCopy[0];

        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            IDamage c = colliderCopy[i].GetComponent<IDamage>();
            if (c != null)
            {
                float dist1 = Vector3.Distance(colliderCopy[i].transform.position, transform.position);
                float dist2 = Vector3.Distance(nearestCollider.transform.position, transform.position);

                if (dist1 < dist2)
                {
                    nearestCollider = colliderCopy[i];
                }
            }
        }

        return nearestCollider;
    }

    public void SetHealth(float val)
    {
        if (GetFriendlyKillerData() == null)
            return;

        GetFriendlyKillerData().SetHealth(GetFriendlyKillerData().GetHealth() - val);
    }

    public float GetHealth()
    {
        if (GetFriendlyKillerData() == null)
            return 0f;

        return GetFriendlyKillerData().GetHealth();
    }

    public float GetMaxHealth()
    {
        if (GetFriendlyKillerData() == null)
            return 1f;

        return GetFriendlyKillerData().GetMaxHealth();
    }

    public ElementalReaction GetElementalReaction()
    {
        return null;
    }

    public float GetATK()
    {
        return 0f;
    }

    public float GetEM()
    {
        return 0f;
    }
}
