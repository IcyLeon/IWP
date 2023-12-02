using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalReaction
{
    private GameObject source;
    private List<Elements> ElementsList;
    private ElementalReactionsManager elementalReactionsManager;
    public delegate void OnElementChanged(bool isChanged);
    public OnElementChanged onElementChanged;

    public void SetSource(GameObject go)
    {
        source = go;
    }
    public void UpdateElementsList()
    {
        for (int i = 0; i < ElementsList.Count; i++)
        {
            Elements elements = ElementsList[i];
            if (elements.GetActive())
            {
                elements.UpdateElementalEffectTime();
            }
            else
            {
                ElementsList.Remove(elements);
                i--;
            }
        }
    }

    private ElementalReactionsInfo GetElementalReactionsInfo(List<Elements> Elementals)
    {
        if (elementalReactionsManager == null)
            return null;

        return elementalReactionsManager.GetElementalReactionState(Elementals);
    }

    public ElementalReactionsTrigger GetElementalReactionsTrigger(Vector3 position)
    {
        ElementalReactionsInfo ElementalReactionsInfo = GetElementalReactionsInfo(GetElementList());
        if (ElementalReactionsInfo == null)
            return null;

        ElementalReactionsTrigger trigger = new ElementalReactionsTrigger();
        trigger.SetERState(ElementalReactionsInfo.elementalReactionState);

        AssetManager.GetInstance().SpawnWorldText_ElementalReaction(position, trigger.GetERState(), ElementalReactionsManager.GetInstance().GetElementalReactionText(trigger.GetERState()));
        GetElementList().Clear();

        return trigger;
    }



    public void AddElements(Elements elements)
    {
        if (elements == null)
            return;

        if (elements.GetElements() == Elemental.NONE)
            return;

        Elements ExistingElements = isElementsAlreadyExisted(elements);

        if (ExistingElements == null)
        {
            elements.ResetElementalEffect();
            ElementsList.Add(elements);
        }
        else
        {
            ExistingElements.ResetElementalEffect();
        }

        onElementChanged?.Invoke(ExistingElements == null);
    }

    public Elements isElementsAlreadyExisted(Elements e)
    {
        for (int i = 0; i < ElementsList.Count; i++)
        {
            Elements elements = ElementsList[i];
            if (elements.GetElements() == e.GetElements())
            {
                return elements;
            }
        }
        return null;
    }

    public Elements isElementsAlreadyExisted(Elemental e)
    {
        for (int i = 0; i < ElementsList.Count; i++)
        {
            Elements elements = ElementsList[i];
            if (elements.GetElements() == e)
            {
                return elements;
            }
        }
        return null;
    }


    public List<Elements> GetElementList()
    {
        return ElementsList;
    }

    public ElementalReaction()
    {
        ElementsList = new List<Elements>();
        elementalReactionsManager = ElementalReactionsManager.GetInstance();
    }
}
