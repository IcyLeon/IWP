using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionContentUI : MonoBehaviour
{
    [SerializeField] Transform InteractOptionsPivot;
    [SerializeField] GameObject SelectionArrowPrefab;
    [SerializeField] GameObject InteractablePrefab;
    private Dictionary<IInteract, InteractOptions> InteractOptionList;
    [SerializeField] PlayerCanvasUI playerCanvasUI;
    private GameObject SelectionArrow;
    private int CurrentIdx;

    private void Awake()
    {
        CurrentIdx = -1;
        InteractOptionList = new();
        PlayerController.OnInteract += InteractObj;
        PlayerController.OnScroll += Scroll;
        InteractManager.OnInteractAdd += OnInteractAdd;
        InteractManager.OnInteractRemove += OnInteractRemove;
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

    private void InteractObj()
    {
        if (CurrentIdx == -1)
            return;

        IInteract interactObj = InteractOptionList.ElementAt(CurrentIdx).Key;
        if (interactObj != null)
        {
            interactObj.Interact(playerCanvasUI.GetPlayerManager());
        }
    }

    private void OnInteractionListChanged()
    {
        CurrentIdx = 0;

        UpdateArrowPosition();

        if (InteractOptionList.Count == 0)
            CurrentIdx = -1;
    }

    private void UpdateArrowPosition()
    {

        if (InteractOptionList.Count != 0)
        {
            if (SelectionArrow == null)
                SelectionArrow = playerCanvasUI.GetInteractOptionsUI().SpawnInteractionArrow();
        }
        else
        {
            if (SelectionArrow != null)
                Destroy(SelectionArrow.gameObject);
        }

        if (CurrentIdx == -1 || InteractOptionList.Count == 0)
            return;

        Vector2 position = InteractOptionList.ElementAt(CurrentIdx).Value.GetComponent<RectTransform>().anchoredPosition;
        SelectionArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-35f, position.y);
    }

    private void OnInteractAdd(IInteract interact)
    {
        InteractOptions InteractOptions = SpawnInteractObj(interact);
        InteractOptionList.Add(interact, InteractOptions);
        OnInteractionListChanged();
    }

    private void OnInteractRemove(IInteract interact)
    {
        if (InteractOptionList.ContainsKey(interact))
        {
            Destroy(InteractOptionList[interact].gameObject);
            InteractOptionList.Remove(interact);
            OnInteractionListChanged();
        }
    }

    private void OnDestroy()
    {
        PlayerController.OnInteract -= InteractObj;
        PlayerController.OnScroll -= Scroll;
        InteractManager.OnInteractAdd -= OnInteractAdd;
        InteractManager.OnInteractRemove -= OnInteractRemove;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateArrowPosition();
    }

    private InteractOptions SpawnInteractObj(IInteract interactable)
    {
        InteractOptions interactOptions = Instantiate(InteractablePrefab, playerCanvasUI.GetInteractOptionsUI().GetInteractOptionsPivot().transform).GetComponent<InteractOptions>();
        interactOptions.SetIInferact(interactable);
        interactOptions.UpdateText(interactable.InteractMessage());
        return interactOptions;
    }

    public Transform GetInteractOptionsPivot()
    {
        return InteractOptionsPivot;
    }

    public GameObject SpawnInteractionArrow()
    {
        GameObject go = Instantiate(SelectionArrowPrefab, transform);
        return go;
    }
}
