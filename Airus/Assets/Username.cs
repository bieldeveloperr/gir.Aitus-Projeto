using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Username : MonoBehaviour
{
    [SerializeField] PhotonView PLayerView;
    [SerializeField] TextMeshProUGUI Name;

    void Start() 
    {
        if (PLayerView.IsMine)
            Destroy(this.gameObject);

        Name.text = PLayerView.Owner.NickName;
    }
}
