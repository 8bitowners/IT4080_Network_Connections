using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class NetworkPlayerScore : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text playerName;
    [SerializeField]
    TMPro.TMP_Text scoreUI;
    [SerializeField]
    TMPro.TMP_Text playerNum;

    public void TrackPlayer(GameObject player) {
        player.GetComponent<NetwrokPlayerData>().playerName.OnValueChanged += OnNameChanged;
        player.GetComponent<NetwrokPlayerData>().score.OnValueChanged += OnScoreChanged;
        player.GetComponent<NetwrokPlayerData>().playerNumber.OnValueChanged += OnPlayerNumChanged;

        OnScoreChanged(0, player.GetComponent<NetwrokPlayerData>().score.Value);
        OnNameChanged("", player.GetComponent<NetwrokPlayerData>().playerName.Value);
        OnPlayerNumChanged(0, player.GetComponent<NetwrokPlayerData>().playerNumber.Value);
    }

    public void OnPlayerNumChanged(ulong previousValue, ulong newValue) {
        playerNum.text = newValue.ToString();
    }
    public void OnNameChanged(FixedString128Bytes previousValue, FixedString128Bytes newValue) {
        playerName.text = newValue.ToString();
    }
    public void OnScoreChanged(int previousValue, int newValue) {
        scoreUI.text = newValue.ToString();
    } 
}
