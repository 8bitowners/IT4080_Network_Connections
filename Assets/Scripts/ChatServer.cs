using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChatServer : NetworkBehaviour
{
    public ChatUi chatUi; 
    const ulong SYSTEM_ID = ulong.MaxValue; 
    private ulong[] dmClientIds = new ulong[2];
    private List<ulong> connectedUsers = new List<ulong>();
    


    void Start()
    {
        chatUi.printEnteredText = false;    
        chatUi.MessageEntered += OnChatUiMessageEntered;

        if (IsServer) {
            NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
            NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
            if (IsHost) {
                DisplayMessageLocally(SYSTEM_ID, $"You are the host AND client {NetworkManager.LocalClientId}");
                connectedUsers.Add(NetworkManager.LocalClientId);
            } else {
                DisplayMessageLocally(SYSTEM_ID, "You are the server");
            }
        } else {
            DisplayMessageLocally(SYSTEM_ID, $"You are client {NetworkManager.LocalClientId}");
        }
    }

    private void ServerOnClientConnected(ulong clientId) {
        connectedUsers.Add(clientId);

        ServerSendDirectMessage("Welcome to the server!", SYSTEM_ID, clientId);
        ReceiveChatMessageClientRpc($"Client {clientId} has connected to the server", SYSTEM_ID);

        string serverContents = "The current server list is: ";
        foreach (var x in connectedUsers) {
            serverContents += ", " + x.ToString();
            //the method for printing this could be way better, but this was just me making sure it printed correctly and likely isn't in the final submission anyways
        }
        
        ReceiveChatMessageClientRpc(serverContents, SYSTEM_ID);

    }

    private void ServerOnClientDisconnected(ulong clientId) {
        ReceiveChatMessageClientRpc($"Client {clientId} has disconnected from the server", SYSTEM_ID);
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
        if (message.StartsWith("@")) {
            string[] parts = message.Split(" ");
            string clientIdStr = parts[0].Replace("@", "");
            ulong toClientId = ulong.Parse(clientIdStr);

            ServerSendDirectMessage(message, serverRpcParams.Receive.SenderClientId, toClientId);

        } else {
            ReceiveChatMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);
        }
        
        
    }

    [ClientRpc] 
    public void ReceiveChatMessageClientRpc(string message, ulong from, ClientRpcParams clientRpcParams = default) {
        DisplayMessageLocally(from , message);
    }

    private void ServerSendDirectMessage(string message, ulong from, ulong to) {
        
        bool targetExists = connectedUsers.Contains(to);
        
        if (targetExists == true) {
            dmClientIds[0] = from;
            dmClientIds[1] = to; 

            ClientRpcParams rpcParams = default; 
            rpcParams.Send.TargetClientIds = dmClientIds; 

            ReceiveChatMessageClientRpc($"<whisper> {message}", from, rpcParams);
        }
        else {
            ServerSendDirectMessage("Invalid target ID", SYSTEM_ID, from);
        } 
       
    }
}
