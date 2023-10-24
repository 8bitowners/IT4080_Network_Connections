using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLobby : MonoBehaviour
{

    public LobbyUi lobbyUi;

    void Start()
    {
        CreateTestCards();
        lobbyUi.OnReadyToggled += TestOnReadyToggled; 
        lobbyUi.OnStartClicked += TestOnStartClicked;
        lobbyUi.ShowStart(true);
        lobbyUi.OnChangeNameClicked += TestOnChangeNameClicked;
    }

    private void CreateTestCards() {
        PlayerCard pc = lobbyUi.playerCards.AddCard("test player 1"); 
        pc.color = Color.grey; 
        pc.ready = true; 
        pc.ShowKick(true);
        pc.clientId = 00;
        pc.OnKickClicked += TestOnKickClicked; 
        pc.UpdateDisplay(); 
    }

    private void TestOnKickClicked(ulong clientId) {
        Debug.Log($"We wanna kick client {clientId}"); 
    }

    private void TestOnReadyToggled(bool newValue) {
        Debug.Log($"Ready = {newValue}"); 
    }

    private void TestOnStartClicked() {
        lobbyUi.ShowStart(false);
    }

    private void TestOnChangeNameClicked(string newName) {
        Debug.Log($"new name = {newName}"); 
    }
   
}
