using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SalvarControles : MonoBehaviour
{
    [SerializeField] Slider Sensibility;

    public void OnSaveControls()
    {
        PlayerPrefs.SetFloat("Sensi_Cam", Sensibility.value);
    }
}
