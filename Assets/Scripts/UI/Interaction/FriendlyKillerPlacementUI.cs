using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyKillerPlacementUI : MonoBehaviour
{
    [SerializeField] Image FriendlyKillerImage;
    [SerializeField] TextMeshProUGUI FriendlyKillerName;
    [SerializeField] HealthBarScript HealthBarScript;
    private FriendlyKillerData FriendlyKillerData;

    private void Start()
    {
        HealthBarScript.Init(false, true);
    }

    public void SetFriendlyKillerREF(FriendlyKillerData f)
    {
        FriendlyKillerData = f;

        if (FriendlyKillerData != null)
        {
            FriendlyKillerName.text = FriendlyKillerData.GetFriendlyKillerSO().PurchaseableObjectName;
            FriendlyKillerImage.sprite = FriendlyKillerData.GetFriendlyKillerSO().PurchaseableObjectSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FriendlyKillerData == null)
            return;

        HealthBarScript.UpdateHealth(FriendlyKillerData.GetHealth(), 0f, FriendlyKillerData.GetMaxHealth());
    }
}
