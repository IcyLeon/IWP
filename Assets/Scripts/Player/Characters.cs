using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamage {
    Vector3 GetPointOfContact();
    bool IsDead();
    Elements TakeDamage(Vector3 position, Elements elements, float damageAmt, bool callHitInfo = true);
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

    public delegate void onElementReactionHit(ElementalReactionsTrigger e);
    public onElementReactionHit OnElementReactionHit;

    protected Coroutine DieCoroutine;
    protected bool isAttacking;
    protected virtual void Start()
    {
        BaseMaxHealth = CharactersSO.GetAscensionInfo(0).BaseHP;
        isAttacking = false;
    }
    public void ResetAttack()
    {
        isAttacking = false;
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
            healthBarScript.SetupMinAndMax(0, GetMaxHealth());
            healthBarScript.UpdateHealth(GetHealth());
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
        if (GetHealth() <= 0)
            return true;

        return false;
    }

    public virtual Elements TakeDamage(Vector3 pos, Elements elementsREF, float amt, bool callHitInfo = true)
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

        if (GetElementalReaction() != null)
            GetElementalReaction().AddElements(e);

        ElementalReactionsTrigger ElementalReactionsTrigger = GetElementalReaction().GetElementalReactionsTrigger(pos);
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
            OnElementReactionHit?.Invoke(ElementalReactionsTrigger);
        }
        SetHealth(GetHealth() - amt);
        if (callHitInfo)
            OnHit(e, this);
        UpdateDie();
        AssetManager.GetInstance().SpawnWorldText_Elemental(pos, elemental, amt.ToString());

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
}
