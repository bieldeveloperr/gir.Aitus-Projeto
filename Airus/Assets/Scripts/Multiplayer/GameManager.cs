using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using Player.System.Movement;
using Player.System.Hud;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

namespace Player.System.Multiplayer
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Core Methods

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            base.OnConnectedToMaster();
        }

        public override void OnJoinedLobby()
        {
            SceneManager.LoadScene("SceneTestes");
            base.OnJoinedLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
        }
        #endregion

        void OnGUI() 
        {
            var InfoText = "Informações do Servidor:\n";
            InfoText += string.Format("Players: {0}\n Ping {1}", PhotonNetwork.PlayerList.Length, PhotonNetwork.GetPing());

            GUILayout.BeginVertical("box");
            GUILayout.Label(InfoText);
            GUILayout.EndVertical();
        }
    }
}
