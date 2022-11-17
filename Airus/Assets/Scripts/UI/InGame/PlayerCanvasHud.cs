using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Player.System.CameraMovement;
using Player.System.Movement;
using UnityEngine.SceneManagement;

namespace Player.System.Canvas.Settings
{
    public class PlayerCanvasHud : MonoBehaviour
    {
        #region Variables
        Player_Camera Player_Camera;

        [SerializeField] GameObject MenuPause;
        #endregion

        #region Core Methods
        void Awake() {}

        void Start()
        {
            Player_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera>();
        }

        void Update()
        {
            InputCanvas();
        }
        #endregion

        #region General Methods
        void InputCanvas()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !MenuPause.activeInHierarchy) PauseGame();
        }

        void PauseGame()
        {
            MenuPause.SetActive(true);

            Player_Camera._CanMove = false;
            Player_Camera._HideMouse = false;

            Player_Camera.Target.GetComponent<Player_Movement>()._CanMove = false;
        }

        public void Resumir()
        {
            MenuPause.SetActive(false);

            Player_Camera._CanMove = true;
            Player_Camera._HideMouse = true;

            Player_Camera.Target.GetComponent<Player_Movement>()._CanMove = true;
        }

        public void Desconectar()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Menu");
        }
        #endregion
    }
}
