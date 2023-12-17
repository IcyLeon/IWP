using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] CinemachineVirtualCamera playerCamera;
    [SerializeField] CinemachineVirtualCamera aimCamera;
    private Coroutine ScreenShakeCoroutine;

    public GameObject GetAimCamera()
    {
        return aimCamera.gameObject;
    }

    private IEnumerator ScreenShake(float frequency, float amptitude, float timer)
    {
        CinemachineBasicMultiChannelPerlin VirtualCameraPerlin = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CinemachineBasicMultiChannelPerlin AimCameraPerlin = aimCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        VirtualCameraPerlin.m_AmplitudeGain = amptitude;
        VirtualCameraPerlin.m_FrequencyGain = frequency;
        AimCameraPerlin.m_AmplitudeGain = VirtualCameraPerlin.m_AmplitudeGain;
        AimCameraPerlin.m_FrequencyGain = VirtualCameraPerlin.m_FrequencyGain;
        float elapsed = 0;

        while (elapsed < timer)
        {
            VirtualCameraPerlin.m_FrequencyGain = Mathf.Lerp(VirtualCameraPerlin.m_FrequencyGain, 0f, elapsed / timer);
            AimCameraPerlin.m_FrequencyGain = VirtualCameraPerlin.m_FrequencyGain;
            elapsed += Time.deltaTime;
            yield return null;
        }

        VirtualCameraPerlin.m_FrequencyGain = 0f;
        AimCameraPerlin.m_FrequencyGain = VirtualCameraPerlin.m_FrequencyGain;
    }

    public void CameraShake(float frequency, float amptitude, float timer)
    {
        if (ScreenShakeCoroutine != null)
            StopCoroutine(ScreenShakeCoroutine);

        ScreenShakeCoroutine = StartCoroutine(ScreenShake(frequency, amptitude, timer));
    }

    public static bool CheckIfInCameraView(Vector3 pos)
    {
        Vector3 viewPoint = Camera.main.WorldToViewportPoint(pos);
        return viewPoint.z >= Camera.main.nearClipPlane && viewPoint.z <= Camera.main.farClipPlane &&
                viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1;
    }

    private void LockWhenPaused()
    {
        CinemachinePOV playerPOV = playerCamera.GetCinemachineComponent<CinemachinePOV>();
        CinemachinePOV aimPOV = aimCamera.GetCinemachineComponent<CinemachinePOV>();

        if (!MainUI.GetInstance().isCursorVisible())
        {
            if (playerPOV != null)
            {
                playerPOV.m_HorizontalAxis.m_InputAxisName = "Mouse X";
                playerPOV.m_VerticalAxis.m_InputAxisName = "Mouse Y";
            }
            if (aimPOV != null)
            {
                aimPOV.m_HorizontalAxis.m_InputAxisName = "Mouse X";
                aimPOV.m_VerticalAxis.m_InputAxisName = "Mouse Y";
            }
        }
        else
        {
            if (playerPOV != null)
            {
                playerPOV.m_HorizontalAxis.m_InputAxisName = "";
                playerPOV.m_VerticalAxis.m_InputAxisName = "";
                playerPOV.m_HorizontalAxis.m_InputAxisValue = 0f;
                playerPOV.m_VerticalAxis.m_InputAxisValue = 0f;
            }
            if (aimPOV != null)
            {
                aimPOV.m_HorizontalAxis.m_InputAxisName = "";
                aimPOV.m_VerticalAxis.m_InputAxisName = "";
                aimPOV.m_HorizontalAxis.m_InputAxisValue = 0f;
                aimPOV.m_VerticalAxis.m_InputAxisValue = 0f;
            }
        }
        //playerCamera.enabled = aimCamera.enabled = !MainUI.GetInstance().isCursorVisible();
    }

    private void UpdateCamera()
    {
        if (playerController.GetPlayerManager().GetCharacterRB() == null)
            return;

        playerCamera.Follow = playerController.GetPlayerManager().GetPlayerOffsetPosition();
        aimCamera.Follow = playerCamera.Follow;
        LockWhenPaused();
        //Vector3 d = testCamera.LookAt.position - (playerController.GetPlayerOffsetPosition().position);
        //d.Normalize();
        //testCamera.m_XAxis.Value = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg - 15f;
    }
    public void Recentering()
    {
        StartCoroutine(RecenteringCoroutine());
    }
    private IEnumerator RecenteringCoroutine()
    {
        CinemachinePOV playerPOV = playerCamera.GetCinemachineComponent<CinemachinePOV>();

        playerPOV.m_HorizontalRecentering.m_enabled = true;
        yield return null;
        playerPOV.m_HorizontalRecentering.m_enabled = false;
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
