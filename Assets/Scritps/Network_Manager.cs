using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Network_Manager : MonoBehaviourPun
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            base.photonView.RPC("RPCShuffle", Photon.Pun.RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPCShuffle()
    {
        Game.Instance.Shuffle();
    }
}
