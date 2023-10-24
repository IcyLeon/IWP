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
    protected int Level;
    [SerializeField] protected CharactersSO CharactersSO;
    protected HealthBarScript healthBarScript;
    private ElementalReaction elementalReaction;

    protected virtual void Start()
    {
        BaseMaxHealth = CharactersSO.BaseHP;
        elementalReaction = new ElementalReaction();
    }

    protected virtual void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.SetupMinAndMax(0, GetMaxHealth());
            healthBarScript.UpdateContent(GetHealth(), GetLevel());
        }

        if (elementalReaction != null)
            elementalReaction.UpdateElementsList();
    }

    private Rigidbody GetRB()
    {
        if (this is PlayerCharacters)
        {
            return CharacterManager.GetInstance().GetPlayerController().GetCharacterRB();
        }
        return transform.GetComponent<Rigidbody>();
    }

    public void TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        Elemental elemental;
        if (elements != null)
            elemental = elements.GetElements();
        else
            elemental = Elemental.NONE;

        if (elementalReaction != null)
            elementalReaction.AddElements(elements);

        ElementalReactionsInfo e = elementalReaction.GetElementalReactionsInfo(elementalReaction.GetElementsList());
        if (e != null)
        {
            switch(e.elementalReactionState)
            {
                case ElementalReactionState.OVERLOAD:
                    GetRB().AddForce(Vector3.up * 8f, ForceMode.Impulse);
                    break;
                case ElementalReactionState.MELT:
                    break;
            }
            AssetManager.GetInstance().SpawnWorldText_ElementalReaction(transform.position, e.elementalReactionState, ElementalReactionsManager.GetInstance().GetElementalReactionText(e.elementalReactionState));
            
            elementalReaction.GetElementsList().Clear();
        }

        AssetManager.GetInstance().SpawnWorldText_Elemental(pos, elemental, amt.ToString());
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
}
