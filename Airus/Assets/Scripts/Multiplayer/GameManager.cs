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

        float current = 0;
        int avgFrameRate;
        private void Update() 
        {
            current = Time.frameCount / Time.time;
            avgFrameRate = (int)current;
        }
        #endregion

        void OnGUI() 
        {
            var InfoText = "Informações do Servidor:\n";
            InfoText += string.Format("Players: {0}\nPing {1}\nFPS: {2}", PhotonNetwork.PlayerList.Length, PhotonNetwork.GetPing(), avgFrameRate);

            GUILayout.BeginVertical("box");
            GUILayout.Label(InfoText);
            GUILayout.EndVertical();
        }
    }
}
