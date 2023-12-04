using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WorldTextREF;

    public void UpdateContent(string val)
    {
        WorldTextREF.text = val;
    }
}
