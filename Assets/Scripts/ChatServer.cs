using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChatServer : NetworkBehaviour
{
    public ChatUi chatUi; 
    const ulong SYSTEM_ID = ulong.MaxValue; 


    void Start()
    {
        chatUi.printEnteredText = false;    
        chatUi.MessageEntered += OnChatUiMessageEntered;

        if (IsServer) {
            if (IsHost) {
                DisplayMessageLocally(SYSTEM_ID, $"You are the host AND client {NetworkManager.LocalClientId}");
            } else {
                DisplayMessageLocally(SYSTEM_ID, "You are the server");
            }
        } else {
            DisplayMessageLocally(SYSTEM_ID, $"You are client {NetworkManager.LocalClientId}");
        }
    }

    private void DisplayMessageLocally(ulong from, string message) {
        string fromStr = $"Player {from}";
        Color textColor = chatUi.defaultTextColor;

    if(from == NetworkManager.LocalClientId) {
        fromStr = "you"; 
        textColor = Color.magenta;
    } else if(from == SYSTEM_ID) {
        fromStr = "SYS";
        textColor = Color.green; 
    }
        chatUi.addEntry(fromStr, message);
    }

    private void OnChatUiMessageEntered(string message) {
        SendChatMessageServerRpc(message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default) {
        ReceiveChatMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc] 
    public void ReceiveChatMessageClientRpc(string message, ulong from, ClientRpcParams clientRpcParams = default) {
        DisplayMessageLocally(from , message);
    }

}
