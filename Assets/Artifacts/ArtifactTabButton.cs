using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArtifactTabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ArtifactType ArtifactType;
    [SerializeField] ArtifactTabGroup tabGroup;
    [SerializeField] GameObject onlineBorder;
    [SerializeField] Image tabIcon;

    public ArtifactType artifactType
    {
        get { return ArtifactType;  }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        tabGroup.Subscribe(this);
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


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tabGroup.GetTabSelected() != this)
            HighlightTabIcons();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tabGroup.GetTabSelected() != this)
            ResetTabIcons();
    }
}
