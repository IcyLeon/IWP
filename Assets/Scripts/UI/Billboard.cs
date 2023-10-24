using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private GameObject UICAM;

    private void Awake()
    {
        UICAM = GameObject.FindGameObjectWithTag("MainCamera");
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = UICAM.transform.rotation;
    }
}
