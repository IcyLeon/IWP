using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour
{
    [SerializeField] Slider slider;
    private float velocity;

    public void UpdateStamina(float val)
    {
        slider.value = Mathf.SmoothDamp(slider.value, val, ref velocity, 0.3f);
    }
}
