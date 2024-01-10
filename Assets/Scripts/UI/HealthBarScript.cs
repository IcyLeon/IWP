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
    [SerializeField] Transform ElementsIndicatorTransform;
    private ElementsIndicator ElementsIndicator;

    public ElementsIndicator GetElementsIndicator()
    {
        return ElementsIndicator;
    }

    public void SetElementsIndicator(IDamage IDamage)
    {
        if (IDamage == null)
        {
            if (GetElementsIndicator() != null)
            {
                Destroy(GetElementsIndicator().gameObject);
            }
            return;
        }

        if (GetElementsIndicator() == null && ElementsIndicatorTransform)
        {
            ElementsIndicator = Instantiate(AssetManager.GetInstance().ElementalContainerPrefab, ElementsIndicatorTransform).GetComponent<ElementsIndicator>();
            GetElementsIndicator().transform.SetAsLastSibling();
        }

        GetElementsIndicator().SetIDamage(IDamage);
    }
    public void UpdateHealth(float HealthVal, float min, float max)
    {
        slider.minValue = min;
        slider.maxValue = max;

        slider.value = HealthVal;

        if (HealthTextDisplay)
        {
            HealthTextDisplay.text = Mathf.RoundToInt(slider.value).ToString() + "/" + Mathf.RoundToInt(slider.maxValue).ToString();
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
