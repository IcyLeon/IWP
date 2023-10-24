using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalReaction
{
    private List<Elements> ElementsList;
    private ElementalReactionsManager elementalReactionsManager;

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

    public ElementalReactionsInfo GetElementalReactionsInfo(List<Elements> Elementals)
    {
        if (elementalReactionsManager == null)
            return null;

        return elementalReactionsManager.GetElementalReactionState(Elementals);
    }

    public void AddElements(Elements elements)
    {
        if (elements == null)
            return;

        Elements ExistingElements = isElementsAlreadyExisted(elements);

        if (ExistingElements == null)
            ElementsList.Add(elements);
        else
            ExistingElements.ResetElementalEffect();
    }

    private Elements isElementsAlreadyExisted(Elements e)
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

    public ElementalReaction()
    {
        ElementsList = new List<Elements>();
        elementalReactionsManager = ElementalReactionsManager.GetInstance();
    }

    public List<Elements> GetElementsList()
    {
        return ElementsList;
    }
}
