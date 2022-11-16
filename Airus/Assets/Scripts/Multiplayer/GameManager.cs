using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Player.System.Movement;
using Player.System.Hud;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject PrefabPlayer;
    [SerializeField] GameObject MenuOptions;
    [SerializeField] PD_CameraController m;
    [SerializeField] GameObject MenuCamera;
    [SerializeField] InputField Nome;

    [SerializeField] GameObject MenuzinHud;
    [SerializeField] GameObject Menuzin;
    [SerializeField] GameObject MenuOp;

    GUIStyle style = new GUIStyle();

    void Awake()
    {
        style.alignment = TextAnchor.MiddleLeft;
        style.normal.textColor = Color.white;
    }

    void Start()
    {
        UpdateUI(false);
    }

    bool aa;
    string InfoText;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !aa && q)
        {
            aa = true;
            Menuzin.SetActive(true);

            m._CanMove = false;
            m._HideMouse = false;

            m.Target.GetComponent<Player_Movement>()._CanMove = false;
        }
    }

    public void Resumir()
    {
        aa = false;
        Menuzin.SetActive(false);

        m._CanMove = true;
        m._HideMouse = true;

        m.Target.GetComponent<Player_Movement>()._CanMove = true;
    }

    bool q;
    void UpdateUI(bool InGame)
    {
        if (InGame)
        {
            MenuOptions.SetActive(false);
            m.gameObject.SetActive(true);
            m._HideMouse = true;
            m.a = true;
            m._CanMove = true;
            MenuCamera.SetActive(false);
            q = true;
            MenuzinHud.SetActive(true);
        }
        else
        {
            MenuOptions.SetActive(true);
            m.gameObject.SetActive(false);
            m._HideMouse = false;
            m._CanMove = false;
            MenuCamera.SetActive(true);
            Menuzin.SetActive(false);
            q = false;
            MenuzinHud.SetActive(false);
        }
    }

    public void _Connect()
    {
        if (Nome.text != "")
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NickName = Nome.text;
        }
        else
            Debug.LogWarning("Coloque seu nome");
    }

    public void _Disconnect()
    {
        PhotonNetwork.Disconnect();
        aa = false;
    }

    public void _Quit()
    {
        Application.Quit();
    }

    public void OptionsAbrir()
    {
        MenuOp.SetActive(true);
    }

    public void OptionsFechar()
    {
        MenuOp.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        base.OnConnectedToMaster();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("Sala" + Random.Range(0,1000).ToString());
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        var Player = PhotonNetwork.Instantiate("Player_Multiplayer", Vector3.zero, Quaternion.identity);
        Player.GetComponent<Player_Movement>().enabled = true;
        Player.GetComponent<Player_Hud>().enabled = true;
        UpdateUI(true);
        base.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UpdateUI(false);
        base.OnDisconnected(cause);
    }

    void OnGUI() 
    {
        InfoText = "Informações do Servidor:\n";
        InfoText += string.Format("Players: {0}\n Ping {1}", PhotonNetwork.PlayerList.Length, PhotonNetwork.GetPing());

        GUILayout.BeginVertical("box");
        GUILayout.Label(InfoText, style);
        GUILayout.EndVertical();
    }
}
