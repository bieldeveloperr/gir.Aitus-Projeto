using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.System.CameraMovement;
using Player.System.Movement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Player.System.Canvas.Settings
{
    public class PlayerCanvasHud : MonoBehaviour
    {
        #region Variables
        Player_Camera Player_Camera;

        [SerializeField] GameObject MenuPause;
        [SerializeField] GameObject MenuEmote;
        [Space]
        [SerializeField] GameObject OptionsPainel;
        [Space]
        [SerializeField] GameObject PrincipalButtons;
        [SerializeField] GameObject OptionsButtons;
        [Space]
        [SerializeField] GameObject AudioCanvas;
        [SerializeField] GameObject ControleCanvas;
        [SerializeField] GameObject GraficosCanvas;
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
            EmoteCanvas();
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

        public void Options()
        {
            PrincipalButtons.SetActive(false);

            OptionsButtons.SetActive(true);
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
            SceneManager.LoadScene("Menu");
        }
        #endregion

        #region Fundo opções
        public void AudioOptions()
        {
            OptionsPainel.SetActive(true);
            AudioCanvas.SetActive(true);

            ControleCanvas.SetActive(false);
            GraficosCanvas.SetActive(false);
        }

        public void ControleOptions()
        {
            OptionsPainel.SetActive(true);
            ControleCanvas.SetActive(true);

            AudioCanvas.SetActive(false);
            GraficosCanvas.SetActive(false);
        }

        public void GraficosOptions()
        {
            OptionsPainel.SetActive(true);
            GraficosCanvas.SetActive(true);

            AudioCanvas.SetActive(false);
            ControleCanvas.SetActive(false);
        }

        public void Voltar()
        {
            OptionsPainel.SetActive(false);
            OptionsButtons.SetActive(false);

            PrincipalButtons.SetActive(true);

            AudioCanvas.SetActive(false);
            ControleCanvas.SetActive(false);
            GraficosCanvas.SetActive(false);
        }
        #endregion

        #region EmoteUI
        void EmoteCanvas()
        {
            if (Input.GetKeyDown(KeyCode.B) && !MenuEmote.activeInHierarchy) 
            {   
                MenuEmote.SetActive(true);

                Player_Camera._CanMove = false;
                Player_Camera._HideMouse = false;

                Player_Camera.Target.GetComponent<Player_Movement>()._CanMove = false;
            }

            // if (Input.GetKeyDown(KeyCode.B) && MenuEmote.activeInHierarchy)
            // {
            //     MenuEmote.SetActive(false);

            //     Player_Camera._CanMove = true;
            //     Player_Camera._HideMouse = true;

            //     Player_Camera.Target.GetComponent<Player_Movement>()._CanMove = true;
            // }
        }
        #endregion
    }
}
