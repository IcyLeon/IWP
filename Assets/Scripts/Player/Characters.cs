using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamage {
    Vector3 GetPointOfContact();
    bool IsDead();
    public void SetHealth(float val);
    public float GetHealth();
    public ElementalReaction GetElementalReaction();
    public float GetMaxHealth();
    public float GetATK();
    public float GetEM();
    Elements TakeDamage(Vector3 position, Elements elements, float damageAmt, IDamage source, bool callHitInfo = true);
}

public interface ICoordinateAttack
{
    void UpdateCoordinateAttack();
    bool CoordinateAttackEnded();

    bool CoordinateCanShoot();

}

public interface ISkillsBurstManager
{
    void UpdateISkills();
    void UpdateIBursts();
    bool ISkillsEnded();
    bool IBurstEnded();

}

public class Characters : MonoBehaviour, IDamage
{
    protected float BaseMaxHealth;
    protected float CurrentHealth;
    protected float BaseATK;
    protected float BaseDEF;
    protected float BaseEM;
    protected int Level;

    [SerializeField] protected CharactersSO CharactersSO;
    [SerializeField] protected Animator Animator;
    [SerializeField] protected GameObject Model;
    [SerializeField] protected Transform HealthBarPivotParent;
    protected HealthBarScript healthBarScript;
    protected ElementalReaction elementalReaction;
    private float CurrentElementalShield;


    protected Coroutine DieCoroutine;
    protected bool isAttacking;
    public CharactersSO GetCharactersSO()
    {
        return CharactersSO;
    }
    protected virtual void Start()
    {
        BaseMaxHealth = GetCharactersSO().GetAscensionInfo(0).BaseHP;
        isAttacking = false;
    }
    public void ResetAttack()
    {
        isAttacking = false;
    }
    // Start is called before the first frame update

    public virtual float GetElementalShield()
    {
        return 0f;
    }

    public float GetCurrentElementalShield()
    {
        return CurrentElementalShield;
    }
    public void SetCurrentElementalShield(float val)
    {
        CurrentElementalShield = val;
        CurrentElementalShield = Mathf.Max(0f, CurrentElementalShield);
    }

    public void SetisAttacking(bool value)
    {
        isAttacking = value;
    }

    public bool GetisAttacking()
    {
        return isAttacking;
    }

    public virtual bool IsDead()
    {
        return GetHealth() <= 0;
    }
    public Animator GetAnimator()
    {
        return Animator;
    }

    public GameObject GetModel()
    {
        return Model;
    }

    public static bool ContainsParam(Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) 
                return true;
        }
        return false;
    }

    protected virtual void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.UpdateHealth(GetHealth(), 0, GetMaxHealth());
            healthBarScript.UpdateLevel(GetLevel());
        }
        if (Animator)
        {
            if (ContainsParam(Animator, "isDead")) {
                Animator.SetBool("isDead", IsDead());
            }
        }
    }

    private void OnAnimatorMove()
    {
        if (Animator == null)
        {
            return;
        }

        AnimatorMove(Animator.deltaPosition, Animator.rootRotation);
    }

    public virtual void AnimatorMove(Vector3 deltaPosition, Quaternion rootRotation)
    {
        transform.position += deltaPosition;
        transform.rotation = rootRotation;
    }

    public virtual ElementalReaction GetElementalReaction()
    {
        return elementalReaction;
    }

    private Rigidbody GetRB()
    {
        if (this is PlayerCharacters)
        {
            PlayerCharacters pc = (PlayerCharacters)this;
            return pc.GetPlayerManager().GetCharacterRB();
        }
        return transform.GetComponent<Rigidbody>();
    }

    public virtual bool UpdateDie()
    {
        return IsDead();
    }


    public IDamage[] GetAllNearestCharacters(Vector3 currentPos, float range, LayerMask mask)
    {
        Collider[] colliders = Physics.OverlapSphere(currentPos, range, mask);
        List<IDamage> colliderCopy = new List<IDamage>();

        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage c = colliders[i].GetComponent<IDamage>();
            if (c != null)
            {
                Vector3 dir = currentPos - c.GetPointOfContact();
                if (Physics.Raycast(c.GetPointOfContact(), dir.normalized, out RaycastHit hit, range, ~LayerMask.GetMask("Entity", "Ignore Raycast", "BossEntity"), QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.GetComponent<PlayerCharacters>() == null)
                    {
                        continue;
                    }
                }

                if (c.IsDead())
                {
                    continue;
                }

                colliderCopy.Add(c);
            }
        }
        return colliderCopy.ToArray();
    }

    protected IDamage GetNearestCharacters(Vector3 currentPos, float range, LayerMask mask)
    {
        IDamage[] colliders = GetAllNearestCharacters(currentPos, range, mask);
        if (colliders.Length == 0)
            return null;

        List<IDamage> colliderCopy = new List<IDamage>(colliders);

        IDamage nearestCollider = colliderCopy[0];

        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            IDamage c = colliderCopy[i];
            if (c != null)
            {
                float dist1 = Vector3.Distance(c.GetPointOfContact(), currentPos);
                float dist2 = Vector3.Distance(nearestCollider.GetPointOfContact(), currentPos);

                if (dist1 <= dist2)
                {
                    nearestCollider = colliderCopy[i];
                }
            }
        }

        return nearestCollider;
    }

    public virtual Elements TakeDamage(Vector3 pos, Elements elementsREF, float amt, IDamage source, bool callHitInfo = true)
    {
        if (IsDead())
            return null;

        Elemental elemental;
        if (elementsREF != null)
        {
            elemental = elementsREF.GetElements();
        }
        else
        {
            elemental = Elemental.NONE;
        }
        Elements e = new Elements(elemental);
        int DamageValue = Mathf.RoundToInt(amt);

        if (GetElementalReaction() != null)
            GetElementalReaction().AddElements(e);

        ElementalReactionsTrigger ElementalReactionsTrigger = GetElementalReaction().GetElementalReactionsTrigger(pos, source, this);
        if (ElementalReactionsTrigger != null)
        {
            switch(ElementalReactionsTrigger.GetERState())
            {
                case ElementalReactionState.OVERCLOCKED:
                    GetRB().AddForce((pos - transform.position).normalized * 2f + Vector3.up * 1f, ForceMode.Impulse);
                    break;
                case ElementalReactionState.MELT:
                    
                    break;
            }

        }

        if (callHitInfo)
            OnHit(e, this);

        if (DamageValue > 0)
        {
            SetHealth(GetHealth() - DamageValue);
            AssetManager.GetInstance().SpawnWorldText_Elemental(pos, elemental, DamageValue.ToString());
        }
        UpdateDie();
        return e;
    }

    protected virtual void OnHit(Elements e, IDamage d)
    {

    }


    public virtual float GetHealth()
    {
        return CurrentHealth;
    }

    public virtual float GetMaxHealth()
    {
        return 1;
    }

    public virtual int GetLevel()
    {
        return Level;
    }

    public virtual float GetATK()
    {
        return BaseATK;
    }

    public virtual float GetDEF()
    {
        return BaseDEF;
    }

    public virtual void SetHealth(float val)
    {
        CurrentHealth = val;
    }

    protected virtual void OnDestroy()
    {
    }

    public virtual Vector3 GetPointOfContact()
    {
        return default(Vector3);
    }

    public virtual float GetEM()
    {
        return 0f;
    }
}
