using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOutsideUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject[] TargetUI;
    [SerializeField] GameObject Mask;

    // Update is called once per frame
    void Update()
    {
        ToggleMask();
    }

    void ToggleMask()
    {
        Mask.SetActive(AssetManager.GetInstance().GetDragItem() == null);
    }

    private bool CheckIfHoveredUIObjectContent()
    {
        GameObject hoverObj = GetHoveredObject();

        if (hoverObj == null)
            return false;

        Transform checkTransform = hoverObj.transform;

        while (checkTransform.parent != null)
        {
            if (checkTransform.gameObject == Mask)
                return false;

            if (CheckIfMatched(checkTransform.gameObject))
                return true;
   
            checkTransform = checkTransform.parent;
        }
        return false;
    }

    private GameObject GetHoveredObject()
    {
        List<RaycastResult> RaycastResults = new List<RaycastResult>();
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerEventData, RaycastResults);

        if (RaycastResults.Count > 0)
        {
            return RaycastResults[0].gameObject;
        }

        return null;
    }

    private bool CheckIfMatched(GameObject go)
    {
        for (int i = 0; i < TargetUI.Length; i++)
        {
            if (TargetUI[i] == go)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckOneisActive()
    {
        for (int i = 0; i < TargetUI.Length; i++)
        {
            if (TargetUI[i].activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    void Close()
    {
        // prevent it from calling the for loop
        if (!CheckOneisActive())
            return;

        for(int i = TargetUI.Length-1; i >= 0; i--)
        {
            if (TargetUI[i].activeSelf)
            {
                TargetUI[i].SetActive(false);
                break;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CheckIfHoveredUIObjectContent())
            Close();
    }
}
