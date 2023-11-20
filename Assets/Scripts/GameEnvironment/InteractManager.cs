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
    private MainUI mainUI;
    private PlayerController playerController;
    private float InteractRange = 8f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.OnInteract += InteractObj;
        mainUI = MainUI.GetInstance();
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
                //Vector3 dir = (collider.transform.position - playerPosition).normalized;
                //if (!Physics.Raycast(playerPosition, dir))
                //{
                //    InteractableList.Add(collider);
                //}
                InteractableList.Add(collider);
            }
        }

        return InteractableList.ToArray();
    }
}
