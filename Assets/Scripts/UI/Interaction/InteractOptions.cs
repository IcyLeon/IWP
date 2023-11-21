using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractOptions : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ContentText;
    private IInteract IInteract;

    public void SetIInferact(IInteract i)
    {
        IInteract = i;
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

    public void Subscribe(List<InteractOptions> ListREF)
    {
        if (ListREF == null)
            return;

        ListREF.Add(this);
    }

    public InteractOptions Select() {
        return this;
    }
}
