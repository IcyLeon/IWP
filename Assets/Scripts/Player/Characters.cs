using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage {
    void TakeDamage(Vector3 position, Elements elements, float damageAmt);
}

public class Characters : MonoBehaviour, IDamage
{
    protected float BaseMaxHealth;
    protected float CurrentHealth;
    protected float BaseATK;
    protected float BaseDEF;
    protected int Level;
    [SerializeField] protected CharactersSO CharactersSO;
    [SerializeField] protected Animator Animator;
    protected HealthBarScript healthBarScript;
    private ElementalReaction elementalReaction;

    protected virtual void Start()
    {
        BaseMaxHealth = CharactersSO.BaseHP;
        elementalReaction = new ElementalReaction();
    }

    public Animator GetAnimator()
    {
        return Animator;
    }

    protected virtual void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.SetupMinAndMax(0, GetMaxHealth());
            healthBarScript.UpdateHealth(GetHealth());
            healthBarScript.UpdateLevel(GetLevel());
        }

        if (GetElementalReaction() != null)
            GetElementalReaction().UpdateElementsList();
    }

    public ElementalReaction GetElementalReaction()
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

    public virtual void TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        Elemental elemental;
        if (elements != null)
            elemental = elements.GetElements();
        else
            elemental = Elemental.NONE;

        if (GetElementalReaction() != null)
            GetElementalReaction().AddElements(elements);

        ElementalReactionsTrigger e = GetElementalReaction().GetElementalReactionsTrigger(pos);
        if (e != null)
        {
            switch(e.GetERState())
            {
                case ElementalReactionState.OVERCLOCKED:
                    GetRB().AddForce(Vector3.up * 5f, ForceMode.Impulse);
                    break;
                case ElementalReactionState.MELT:
                    break;
            }
            StartCoroutine(RemoveDelayElementalReaction());
        }
        SetHealth(GetHealth() - amt);
        AssetManager.GetInstance().SpawnWorldText_Elemental(pos, elemental, amt.ToString());
    }

    private IEnumerator RemoveDelayElementalReaction()
    {
        yield return new WaitForSeconds(0.35f);
        elementalReaction.GetElementList().Clear();
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
