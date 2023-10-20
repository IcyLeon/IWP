using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeCanvasTransition : MonoBehaviour, IPointerClickHandler
{
    private Item ItemREF;
    [SerializeField] UpgradeCanvas upgradeCanvas;
    [SerializeField] GameObject ItemShowcaseCanvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        SpawnUI();
    }

    void SpawnUI()
    {
        if (ItemREF == null)
            return;

        upgradeCanvas.OpenUpgradeItemCanvas(ItemREF);
        ItemShowcaseCanvas.SetActive(false);
    }

    public void SetItemREF(Item item)
    {
        ItemREF = item;

        if (ItemREF == null || !(ItemREF is UpgradableItems))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }
}
