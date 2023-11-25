using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour
{
    [SerializeField] Slider slider;

    public void UpdateStamina(float val)
    {
        slider.value = Mathf.Lerp(slider.value, val, Time.deltaTime * 3f);
    }
}
