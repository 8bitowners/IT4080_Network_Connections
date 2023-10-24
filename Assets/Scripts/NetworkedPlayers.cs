using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkedPlayers : NetworkBehaviour
{
    public NetworkList<NetworkPlayerInfo> allNetPlayers; 

    private void Awake() {
        allNetPlayers = new NetworkList<NetworkPlayerInfo>();
    }

    void Start() {
        DontDestroyOnLoad(this.gameObject);
        if (IsServer) {
            ServerStart();
       }
       NetworkHelper.Log($"player count = {allNetPlayers.Count}");
    }

    void ServerStart() {
        NetworkPlayerInfo info = new NetworkPlayerInfo(NetworkManager.LocalClientId);
        info.ready = true; 
        allNetPlayers.Add(info);
    }
}
