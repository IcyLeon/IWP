using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] CinemachineVirtualCamera playerCamera;
    [SerializeField] CinemachineVirtualCamera aimCamera;

    public GameObject GetAimCamera()
    {
        return aimCamera.gameObject;
    }

    private void UpdateCamera()
    {
        if (playerController.GetCharacterRB() == null)
            return;

        playerCamera.Follow = playerController.GetPlayerOffsetPosition();
        //playerCamera.LookAt = playerController.GetPlayerOffsetPosition();
        aimCamera.Follow = playerCamera.Follow;
        //aimCamera.LookAt = playerCamera.LookAt;

        //Vector3 d = testCamera.LookAt.position - (playerController.GetPlayerOffsetPosition().position);
        //d.Normalize();
        //testCamera.m_XAxis.Value = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg - 15f;
    }

    public void CameraDefault()
    {
        playerCamera.gameObject.SetActive(true);
        aimCamera.gameObject.SetActive(false);
    }

    public void CameraAim()
    {
        playerCamera.gameObject.SetActive(false);
        aimCamera.gameObject.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }
}
