using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player.System.CameraMovement;

public class SalvarControlesInGame : MonoBehaviour
{
    [SerializeField] Slider Sensibility;

    Player_Camera mCamera;

    void Start() 
    {
        mCamera = Camera.main.GetComponent<Player_Camera>();
    }

    public void OnSaveControls()
    {
        PlayerPrefs.SetFloat("Sensi_Cam", Sensibility.value);
        mCamera.SetConfigs();
    }
}
