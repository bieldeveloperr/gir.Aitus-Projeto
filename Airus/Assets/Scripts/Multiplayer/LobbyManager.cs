using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Player.System.Multiplayer
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        #region Variables
        [SerializeField] InputField NickName;
        [Space]
        [SerializeField] Button Connect;
        [SerializeField] Button Quit;
        #endregion

        #region General Methods
        public void _Connect()
        {   
            if (NickName.text != "")
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.NickName = NickName.text;
            }
            else
                Debug.LogWarning("Coloque seu nome");
        }

        public void _QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}
