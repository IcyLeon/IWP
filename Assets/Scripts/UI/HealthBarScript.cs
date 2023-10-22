using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI HealthTextDisplay;
    [SerializeField] TextMeshProUGUI LevelTextDisplay;
    public void UpdateContent(float HealthVal, int Level, bool DisplayText = true)
    {
        if (HealthTextDisplay)
        {
            HealthTextDisplay.gameObject.SetActive(DisplayText);
            HealthTextDisplay.text = HealthVal.ToString() + "/" + slider.maxValue.ToString();
        }

        if (LevelTextDisplay)
        {
            LevelTextDisplay.text = "Lv." + Level.ToString();
        }
        slider.value = HealthVal;
    }

    public void SetupMinAndMax(float min, float max)
    {
        slider.minValue = min;
        slider.maxValue = max;
    }
}
