using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.System.CameraMovement;
using Photon.Pun;
using Player.System.Movement;
using Player.System.Hud;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

namespace Player.System.Multiplayer
{
    public class SpawnPlayers_GameManager : MonoBehaviourPunCallbacks
    {
        #region Variables
        Player_Camera CameraSystem;
        [SerializeField] GameObject PlayerPrefab;
        [SerializeField] GameObject SpawnPoint;
        #endregion

        #region Core Methods
        void Awake()
        {
            CameraSystem = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera>();
            PhotonNetwork.JoinRandomOrCreateRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom("Sala" + Random.Range(0000, 1000).ToString());
            base.OnJoinRandomFailed(returnCode, message);
        }

        public override void OnJoinedRoom()
        {
            SpawnPlayers();
            base.OnJoinedRoom();
        }
        #endregion

        #region General Methods
        void SpawnPlayers()
        {
            GameObject Player = (GameObject)PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoint.transform.position, Quaternion.identity);

            Player.GetComponent<Player_Movement>().enabled = true;
            Player.GetComponent<CharacterController>().enabled = true;
            Player.GetComponent<Player_Hud>().enabled = true;
            Player.layer = 6;

            CameraSystem.SetCameraTarget(Player.transform);
        }
        #endregion
    }
}
