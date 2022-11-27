using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderCount : MonoBehaviour
{
    [SerializeField] string Prefs;
    [SerializeField] float ResetValue;
    [Space]
    [SerializeField] TextMeshProUGUI Value;
    Slider Slider;

    public void Awake()
    {
        Slider = GetComponent<Slider>();

        if (PlayerPrefs.HasKey(Prefs))
            Slider.value = PlayerPrefs.GetFloat(Prefs);
        else
            Slider.value = ResetValue;
    }

    public void OnSliderCount(float value)
    {
        Value.text = value.ToString();
    }
}
