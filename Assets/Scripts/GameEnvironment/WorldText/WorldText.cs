using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WorldTextREF;
    [SerializeField] Image Icon;
    public void UpdateContent(Sprite icon, string val)
    {
        Icon.sprite = icon;
        WorldTextREF.text = val;
    }
}
