using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Canvas.UI.Manager
{
    public class UIManager_Lobby : MonoBehaviour
    {
        #region Variables
        [SerializeField] GameObject OptionsPainel;
        [Space]
        [SerializeField] GameObject PrincipalButtons;
        [SerializeField] GameObject OptionsButtons;
        [Space]
        [SerializeField] GameObject AudioCanvas;
        [SerializeField] GameObject ControleCanvas;
        [SerializeField] GameObject GraficosCanvas;
        [Space]
        [SerializeField] Slider SliderSensi;
        #endregion

        #region Fundo Principal
        public void PlayButton()
        {
            SceneManager.LoadScene(1);
        }

        public void Options()
        {
            PrincipalButtons.SetActive(false);

            OptionsButtons.SetActive(true);
        }

        public void Quit()
        {
            Application.Quit();
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
    }
}
