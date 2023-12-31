
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System;
public class NetwrokPlayerData : NetworkBehaviour
{
    //I tried to make this class be called "NetworkPlayerData" instead of "Netwrok" but Unity wasn't recognizing that class name in other classes, but upon
    //changing the name it instantly worked. Maybe I just messed up somehow initially but it works and I'm leaving it
    public NetworkVariable<int> score = new NetworkVariable<int>();
    public NetworkVariable<ulong> playerNumber = new NetworkVariable<ulong>();
    public NetworkVariable<FixedString128Bytes> playerName = new
    NetworkVariable<FixedString128Bytes>();
    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!IsServer) return;
        score.Value = 0;
        playerNumber.Value = GetComponent<NetworkObject>().OwnerClientId;
        GetNameClientRPC(
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]
    {GetComponent<NetworkObject>().OwnerClientId}
                }
            }
        );
    }
    [ClientRpc]
    private void GetNameClientRPC(ClientRpcParams clientRpcParams){
        string[] first = new string[] { "Aldqwe", "Alfqwe", "Ashqwe", "Barn",
        "Blan", "Brack", "Brad", "Brain", "Brom", "Burqwr", "Casqwr", "Chelm", "Clere",
        "Cook", "Dart", "Durqwr", "Edgqwr", "Egwqr", "Elqwr" };
        GetNameServerRPC(first[UnityEngine.Random.Range(0,first.Length-1)]);
    }
    [ServerRpc]
    private void GetNameServerRPC(string pName){
        this.playerName.Value = pName;
    }
}
