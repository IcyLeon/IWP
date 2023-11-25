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

    private void Start()
    {
        slider.onValueChanged.AddListener(UpdateContent);
    }

    private void UpdateContent(float val)
    {
        if (HealthTextDisplay)
        {
            HealthTextDisplay.text = val.ToString() + "/" + slider.maxValue.ToString();
        }
    }

    public void UpdateHealth(float HealthVal)
    {
        slider.value = HealthVal;
    }

    public void UpdateLevel(int Level)
    {
        if (LevelTextDisplay)
        {
            LevelTextDisplay.text = "Lv." + Level.ToString();
        }
    }

    public void SetupMinAndMax(float min, float max)
    {
        slider.minValue = min;
        slider.maxValue = max;
    }

    public void SliderInvsibleOnlyFullHealth()
    {
        slider.gameObject.SetActive(slider.value != slider.maxValue);
    }
}
