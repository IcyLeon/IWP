using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractOptions : MonoBehaviour
{
    [SerializeField] Image InteractIcons;
    [SerializeField] TextMeshProUGUI ContentText;
    private IInteract IInteract;

    public void SetIInferact(IInteract i)
    {
        IInteract = i;
        if (IInteract == null)
            return;

        InteractIcons.sprite = IInteract.GetInteractionSprite();
    }


    public IInteract GetIInferact()
    {
        return IInteract;
    }

    public void UpdateText(string Txt)
    {
        if (ContentText)
            ContentText.text = Txt;
    }
}
