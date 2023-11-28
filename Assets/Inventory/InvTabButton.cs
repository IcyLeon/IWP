using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvTabButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Category CategoryType;
    [SerializeField] InvTabGroup tabGroup;
    [SerializeField] Image tabIcon;
    [SerializeField] GameObject onlineBorder;


    void Awake()
    {
        tabGroup.Subscribe(this);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
    public Category Category
    {
        get { return CategoryType; }
    }

    public void ResetTabIcons()
    {
        var tempColor = Color.white;
        tempColor.a = 0.2f;
        tabIcon.color = tempColor;

        onlineBorder.SetActive(false);
    }

    public void HighlightTabIcons()
    {
        var tempColor = tabIcon.color;
        tempColor.a = 1.0f;
        tabIcon.color = tempColor;
    }

    public void SelectedTabIcons()
    {
        tabIcon.color = new Color32(74, 82, 100, 255);
        onlineBorder.SetActive(true);
    }

}
