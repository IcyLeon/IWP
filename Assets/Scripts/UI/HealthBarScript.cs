using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Slider shieldSlider;
    [SerializeField] TextMeshProUGUI HealthTextDisplay;
    [SerializeField] TextMeshProUGUI LevelTextDisplay;
    [SerializeField] GameObject UiContent;
    [SerializeField] Color32 enemyColor;
    [SerializeField] Color32 allyColor;

    public void UpdateHealth(float HealthVal, float min, float max)
    {
        slider.value = HealthVal;
        slider.minValue = min;
        slider.maxValue = max;

        if (HealthTextDisplay)
        {
            HealthTextDisplay.text = slider.value.ToString() + "/" + slider.maxValue.ToString();
        }
    }

    public void UpdateShield(float val, float min, float max)
    {
        shieldSlider.minValue = min;
        shieldSlider.maxValue = max;
        shieldSlider.value = val;
        shieldSlider.gameObject.SetActive(shieldSlider.value > slider.minValue);
    }

    public void Init(bool showLevel, bool isAlly)
    {
        if (LevelTextDisplay)
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

    public void SliderInvsibleOnlyFullHealth()
    {
        UiContent.SetActive(slider.value != slider.maxValue);
    }
}
