using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalParticleEffects : MonoBehaviour
{
    private IDamage IDamageableObj;
    private Elemental elemental;

    private void Start()
    {
        Characters characters = IDamageableObj as Characters;
        characters.HitInfo += OnHit;
    }

    private void OnHit(Elements e)
    {
        if (IDamageableObj == null)
            return;

        if (e.GetElements() == Elemental.NONE)
            return;

        if (this != null)
            Destroy(gameObject);
    }

    private void Update()
    {
        Characters characters = IDamageableObj as Characters;
        if (characters != null)
        {
            if (characters.GetElementalReaction().isElementsAlreadyExisted(elemental) == null)
                Destroy(gameObject);
        }
    }

    public void SetElemental(Elemental e)
    {
        elemental = e;
    }


    public void SetDamagableObj(IDamage value)
    {
        IDamageableObj = value;
    }

}
