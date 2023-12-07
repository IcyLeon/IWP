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

    private void LockWhenPaused()
    {
        CinemachinePOV playerPOV = playerCamera.GetCinemachineComponent<CinemachinePOV>();
        CinemachinePOV aimPOV = aimCamera.GetCinemachineComponent<CinemachinePOV>();

        if (MainUI.GetInstance().isCursorVisible())
        {
            if (playerPOV != null)
            {
                playerPOV.m_VerticalAxis.m_InputAxisValue = 0;
                playerPOV.m_HorizontalAxis.m_InputAxisValue = 0;
            }
            if (aimPOV != null)
            {
                aimPOV.m_VerticalAxis.m_InputAxisValue = 0;
                aimPOV.m_HorizontalAxis.m_InputAxisValue = 0;
            }
        }
        //playerCamera.enabled = aimCamera.enabled = !MainUI.GetInstance().isCursorVisible();
    }

    private void UpdateCamera()
    {
        if (playerController.GetCharacterRB() == null)
            return;

        playerCamera.Follow = playerController.GetPlayerOffsetPosition();
        aimCamera.Follow = playerCamera.Follow;
        LockWhenPaused();
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
