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

    [SerializeField] Color32 enemyColor;
    [SerializeField] Color32 allyColor;

    public void UpdateHealth(float HealthVal)
    {
        slider.value = HealthVal;

        if (HealthTextDisplay)
        {
            HealthTextDisplay.text = slider.value.ToString() + "/" + slider.maxValue.ToString();
        }
    }

    public void Init(bool showLevel, bool isAlly)
    {
        LevelTextDisplay.gameObject.SetActive(showLevel);
        slider.fillRect.GetComponent<Image>().color = enemyColor;
        if (isAlly)
            slider.fillRect.GetComponent<Image>().color = allyColor;
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
