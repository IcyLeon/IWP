using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IInteract
{
    void Interact();
    string InteractMessage();
}

public class InteractManager : MonoBehaviour
{
    [SerializeField] GameObject InteractablePrefab;
    private MainUI mainUI;
    private PlayerController playerController;
    private float TotalPreviousInteract;
    private float InteractRange = 5f;
    private InteractOptions CurrentInteractOptions;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.OnInteract += InteractObj;
        playerController.OnScroll += Scroll;
        TotalPreviousInteract = 0;
        mainUI = MainUI.GetInstance();

        StartCoroutine(UpdateInteractObj());
    }

    private void Scroll(float val)
    {
        int value = (int)Mathf.Clamp(val, -1f, 1f);

    }

    private void OnDestroy()
    {
        if (playerController)
        {
            playerController.OnInteract -= InteractObj;
            playerController.OnScroll -= Scroll;
        }
    }
    private IEnumerator UpdateInteractObj()
    {
        List<Collider> CurrentInteractList = new();

        while (true)
        {
            Collider[] TotalInteractList = GetAllNearestInteractObj();
            for (int i = 0; i < TotalInteractList.Length; i++)
            {
                if (!CurrentInteractList.Contains(TotalInteractList[i]))
                {
                    CurrentInteractList.Add(TotalInteractList[i]);
                    SpawnInteractObj(TotalInteractList[i]);
                }

            }

            for (int i = CurrentInteractList.Count - 1; i >= 0; i--)
            {
                Collider collider = CurrentInteractList[i];
                if (!TotalInteractList.Contains(collider))
                {
                    RemoveInteractObject(collider);
                    CurrentInteractList.RemoveAt(i);
                }
            }

            yield return null;
        }
    }

    private void RemoveInteractObject(Collider collider)
    {
        foreach(Transform t in mainUI.GetInteractOptionsPivot())
        {
            InteractOptions InteractOptions = t.GetComponent<InteractOptions>();
            if (InteractOptions.GetIInferact() == collider.GetComponent<IInteract>())
                Destroy(t.gameObject);
        }
    }
        
    private void SpawnInteractObj(Collider interactObj)
    {
        IInteract interactable = interactObj.GetComponent<IInteract>();
        InteractOptions interactOptions = Instantiate(InteractablePrefab, mainUI.GetInteractOptionsPivot().transform).GetComponent<InteractOptions>();
        interactOptions.SetIInferact(interactable);
        interactOptions.UpdateText(interactable.InteractMessage());
    }

    private void InteractObj()
    {
        IInteract interactObj = GetNearestInteractObj();
        if (interactObj != null)
        {
            interactObj.Interact();
        }
    }

    private IInteract GetNearestInteractObj()
    {
        Vector3 playerPosition = playerController.GetPlayerOffsetPosition().position;
        Collider[] interactList = GetAllNearestInteractObj();
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

    private Collider[] GetAllNearestInteractObj()
    {
        Vector3 playerPosition = playerController.GetPlayerOffsetPosition().position;
        Collider[] colliders = Physics.OverlapSphere(playerPosition, InteractRange);
        List<Collider> InteractableList = new();

        foreach (Collider collider in colliders)
        {
            IInteract interactable = collider.GetComponent<IInteract>();

            if (interactable != null)
            {

                InteractableList.Add(collider);
            }
        }
        return InteractableList.ToArray();
    }
}
