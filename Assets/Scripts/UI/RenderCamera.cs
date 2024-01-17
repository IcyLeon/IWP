using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCamera : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] string CameraTagName;
    // Start is called before the first frame update
    void Start()
    {
        GameObject UICameraObj = GameObject.FindGameObjectWithTag(CameraTagName);
        if (UICameraObj != null) {
            Camera UICamera = UICameraObj.GetComponent<Camera>();
            canvas.worldCamera = UICamera;
        }

    }
}
