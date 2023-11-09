using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Characters;

public interface IDamage {
    Elements TakeDamage(Vector3 position, Elements elements, float damageAmt);
}

public interface ICoordinateAttack
{
    float GetCoordinateAttackTimer();
    void UpdateCoordinateAttack();
    bool CoordinateAttackEnded();

    bool CoordinateCanShoot();

}

public class Characters : MonoBehaviour, IDamage
{
    protected float BaseMaxHealth;
    protected float CurrentHealth;
    protected float BaseATK;
    protected float BaseDEF;
    protected int Level;

    [SerializeField] protected CharactersSO CharactersSO;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator Animator;
    [SerializeField] protected GameObject Model;
    protected HealthBarScript healthBarScript;
    protected ElementalReaction elementalReaction;
    public delegate void onElementReactionHit(ElementalReactionsTrigger e);
    public onElementReactionHit OnElementReactionHit;
    protected Coroutine DieCoroutine;

    protected virtual void Start()
    {
        BaseMaxHealth = CharactersSO.BaseHP;
    }

    public Animator GetAnimator()
    {
        return Animator;
    }

    public GameObject GetModel()
    {
        return Model;
    }
    protected virtual void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.SetupMinAndMax(0, GetMaxHealth());
            healthBarScript.UpdateHealth(GetHealth());
            healthBarScript.UpdateLevel(GetLevel());
        }
    }

    private void OnAnimatorMove()
    {
        if (!Animator) { return; }

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
            return CharacterManager.GetInstance().GetPlayerController().GetCharacterRB();
        }
        return transform.GetComponent<Rigidbody>();
    }

    public virtual void UpdateDie()
    {

    }
    public virtual Elements TakeDamage(Vector3 pos, Elements elementsREF, float amt)
    {
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
            StartCoroutine(RemoveDelayElementalReaction());
        }
        SetHealth(GetHealth() - amt);
        AssetManager.GetInstance().SpawnWorldText_Elemental(pos, elemental, amt.ToString());

        return e;
    }

    private IEnumerator RemoveDelayElementalReaction()
    {
        yield return new WaitForSeconds(0.3f);
        GetElementalReaction().GetElementList().Clear();
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

    public virtual float GetDamage()
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
}
