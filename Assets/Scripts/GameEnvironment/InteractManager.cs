using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IWorldMarker
{
    Sprite GetWorldMarkerSprite();
    void SpawnMarker();
    Transform GetMarkerParentPivotTransform();
}

public interface IInteract
{
    bool CanInteract();
    void Interact(PlayerManager PM);
    string InteractMessage();
    void OnInteractUpdate(IInteract interactComponent);
    void OnInteractExit(IInteract interactComponent);
    Sprite GetInteractionSprite();
}

public class InteractManager : MonoBehaviour
{
    [SerializeField] GameObject InteractablePrefab;
    private MainUI mainUI;
    private PlayerController playerController;
    private float InteractRange = 3f;
    private List<InteractOptions> InteractOptionList;
    private int CurrentIdx;
    private GameObject SelectionArrow;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        PlayerController.OnInteract += InteractObj;
        PlayerController.OnScroll += Scroll;
        CurrentIdx = -1;
        mainUI = MainUI.GetInstance();
        InteractOptionList = new();
        StartCoroutine(UpdateInteractObj());
    }

    private void Scroll(float val)
    {
        if (val == 0)
            return;

        if (SelectionArrow == null || InteractOptionList.Count == 0)
            return;

        int value = (int)Mathf.Clamp(val, -1f, 1f);
        CurrentIdx -= value;

        UpdateCurrentIdxBound();
    }

    private void UpdateCurrentIdxBound()
    {
        if (CurrentIdx > InteractOptionList.Count - 1)
            CurrentIdx = 0;
        if (CurrentIdx < 0)
            CurrentIdx = InteractOptionList.Count - 1;

    }
    private void Update()
    {
        UpdateArrowPosition();
    }
    private void UpdateArrowPosition()
    {
        if (CurrentIdx == -1 || InteractOptionList.Count == 0)
            return;

        Vector2 position = InteractOptionList[CurrentIdx].GetComponent<RectTransform>().anchoredPosition;
        SelectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-35f, position.y);
    }

    private void OnInteractionListChanged()
    {
        CurrentIdx = 0;

        UpdateArrowPosition();

        if (InteractOptionList.Count == 0)
            CurrentIdx = -1;
    }

    private void OnDestroy()
    {
        PlayerController.OnInteract -= InteractObj;
        PlayerController.OnScroll -= Scroll;
    }

    private IEnumerator UpdateInteractObj()
    {
        List<IInteract> CurrentInteractList = new();

        while (true)
        {
            IInteract[] TotalInteractList = GetAllNearestInteractObj();
            for (int i = 0; i < TotalInteractList.Length; i++)
            {
                if (!CurrentInteractList.Contains(TotalInteractList[i]))
                {
                    TotalInteractList[i].OnInteractUpdate(TotalInteractList[i]);
                    CurrentInteractList.Add(TotalInteractList[i]);
                    SpawnInteractObj(TotalInteractList[i]);
                }
            }

            for (int i = CurrentInteractList.Count - 1; i >= 0; i--)
            {
                IInteract interactObj = CurrentInteractList[i];
                if (!TotalInteractList.Contains(interactObj))
                {
                    interactObj.OnInteractExit(interactObj);
                    RemoveInteractObject(interactObj);
                    CurrentInteractList.RemoveAt(i);
                    OnInteractionListChanged();
                }
            }


            if (TotalInteractList.Length != 0)
            {
                if (SelectionArrow == null)
                    SelectionArrow = mainUI.GetInteractOptionsUI().SpawnInteractionArrow();
            }
            else
            {
                if (SelectionArrow != null)
                    Destroy(SelectionArrow.gameObject);
            }

            yield return null;
        }
    }

    private void RemoveInteractObject(IInteract interactObj)
    {
        foreach(Transform t in mainUI.GetInteractOptionsUI().GetInteractOptionsPivot())
        {
            InteractOptions InteractOptions = t.GetComponent<InteractOptions>();
            if (InteractOptions.GetIInferact() == interactObj)
            {
                InteractOptionList.Remove(InteractOptions);
                Destroy(t.gameObject);
            }
        }
    }
        
    private void SpawnInteractObj(IInteract interactable)
    {
        InteractOptions interactOptions = Instantiate(InteractablePrefab, mainUI.GetInteractOptionsUI().GetInteractOptionsPivot().transform).GetComponent<InteractOptions>();
        interactOptions.SetIInferact(interactable);
        interactOptions.UpdateText(interactable.InteractMessage());
        InteractOptionList.Add(interactOptions);
        UpdateCurrentIdxBound();
    }

    private void InteractObj()
    {
        if (CurrentIdx == -1)
            return;

        IInteract interactObj = InteractOptionList[CurrentIdx].GetIInferact();
        if (interactObj != null)
        {
            interactObj.Interact(playerController.GetPlayerManager());
        }
    }

    private IInteract GetNearestInteractObj()
    {
        Vector3 playerPosition = playerController.GetPlayerManager().GetPlayerOffsetPosition().position;
        Collider[] interactList = GetAllNearestInteractObj() as Collider[];
        IInteract nearestInteractable = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < interactList.Length; i++)
        {
            IInteract interactable = interactList[i].GetComponent<IInteract>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(playerPosition, interactList[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }
        return nearestInteractable;
    }

    private IInteract[] GetAllNearestInteractObj()
    {
        Vector3 playerPosition = playerController.GetPlayerManager().GetPlayerOffsetPosition().position;
        Collider[] colliders = Physics.OverlapSphere(playerPosition, InteractRange);
        List<IInteract> InteractableList = new();

        foreach (Collider collider in colliders)
        {
            IInteract interactable = null;
            GetRootParent r = collider.GetComponent<GetRootParent>();
            if (r != null)
            {
                interactable = r.GetOwner().GetComponent<IInteract>();
            }
            else
            {
                interactable = collider.GetComponent<IInteract>();
            }

            if (interactable != null)
            {
                if (interactable.CanInteract())
                    InteractableList.Add(interactable);
            }
        }
        return InteractableList.ToArray();
    }
}
