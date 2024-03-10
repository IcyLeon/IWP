using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementalReaction
{
    private Dictionary<Elemental, Elements> ElementsDictionary;
    private ElementalReactionsManager elementalReactionsManager;
    public delegate void OnElementChanged(bool isChanged);
    public OnElementChanged onElementChanged;
    private float DisableReactionElasped;
    private const float DisableReactionTime = 0.15f;

    public void UpdateElementsList()
    {
        for (int i = ElementsDictionary.Count; i > ElementsDictionary.Count; i--)
        {
            Elements elements = GetElementsAt(i);
            if (elements != null)
            {
                if (elements.GetActive())
                {
                    elements.UpdateElementalEffectTime();
                }
                else
                {
                    ElementsDictionary.Remove(elements.GetElements());
                }
            }
        }
    }

    private ElementalReactionsInfo GetElementalReactionsInfo(Dictionary<Elemental, Elements> Elementals)
    {
        if (elementalReactionsManager == null)
            return null;

        return elementalReactionsManager.GetElementalReactionState(Elementals);
    }

    public ElementalReactionsTrigger GetElementalReactionsTrigger(Vector3 position, IDamage source, IDamage target)
    {
        ElementalReactionsInfo ElementalReactionsInfo = GetElementalReactionsInfo(GetElementList());
        if (ElementalReactionsInfo == null)
            return null;

        ElementalReactionsTrigger trigger = new ElementalReactionsTrigger();
        trigger.SetERState(ElementalReactionsInfo.elementalReactionState);

        float damage = trigger.CalculateERDamage(source);
        int ActualDmg = Mathf.RoundToInt(damage);
        target.SetHealth(target.GetHealth() - ActualDmg);

        if (damage > 0)
            AssetManager.GetInstance().SpawnWorldText_ElementalReaction(position, trigger.GetERState(), ActualDmg.ToString());

        AssetManager.GetInstance().SpawnWorldText_ElementalReaction(position, trigger.GetERState(), ElementalReactionsManager.GetInstance().GetElementalReactionText(trigger.GetERState()));


        GetElementList().Clear();
        DisableReaction();

        return trigger;
    }

    private void DisableReaction()
    {
        DisableReactionElasped = Time.time;
    }

    private bool CanAddElements()
    {
        return Time.time - DisableReactionElasped > DisableReactionTime;
    }

    public void AddElements(Elements elements)
    {
        if (elements == null)
            return;

        if (elements.GetElements() == Elemental.NONE)
            return;

        if (!CanAddElements())
            return;

        Elements ExistingElements = isElementsAlreadyExisted(elements.GetElements());

        if (ExistingElements == null)
        {
            elements.ResetElementalEffect();
            ElementsDictionary.Add(elements.GetElements(), elements);
        }
        else
        {
            ExistingElements.ResetElementalEffect();
        }

        onElementChanged?.Invoke(ExistingElements == null);
    }

    public Elements isElementsAlreadyExisted(Elemental e)
    {
        if (ElementsDictionary.TryGetValue(e, out var elements))
        {
            return elements;
        }
        return null;
    }

    public Elements GetElementsAt(int i)
    {
        KeyValuePair<Elemental, Elements> elementsKeyValuePair = ElementsDictionary.ElementAt(i);
        if (ElementsDictionary.TryGetValue(elementsKeyValuePair.Key, out Elements elements))
        {
            return elements;
        }
        return null;
    }


    public Dictionary<Elemental, Elements> GetElementList()
    {
        return ElementsDictionary;
    }

    public ElementalReaction()
    {
        ElementsDictionary = new();
        DisableReactionElasped = 0;
        elementalReactionsManager = ElementalReactionsManager.GetInstance();
    }
}
