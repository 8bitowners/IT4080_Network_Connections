using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Arena1Game : NetworkBehaviour
{

    public Player playerPrefab;

    public Player hostPrefab;
    public Camera arenaCamera;

      private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(2, 2, 0),
        new Vector3(-2, 2, 0),
        new Vector3(0, 2, 4),
        new Vector3(0, 2, -4)
    };

     private int colorIndex = 0;
    private Color[] playerColors = new Color[] {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
    };
    
    // Start is called before the first frame update
    void Start()
    {
        arenaCamera.enabled = !IsClient;
        arenaCamera.GetComponent<AudioListener>().enabled = !IsClient;
        if (IsServer) {
             SpawnPlayers();
        }
    }

    private Vector3 NextPosition() {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1) {
            positionIndex = 0;
        }
        return pos;
    }

    private Color NextColor() {
        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if (colorIndex > playerColors.Length - 1) {
            colorIndex = 0;
        }
        return newColor;
    }

    private void SpawnPlayers() { 
        foreach  (ulong clientId in NetworkManager.ConnectedClientsIds) {
            Player prefab = playerPrefab;

            Player playerSpawn = Instantiate( 
                prefab,
                NextPosition(),
                Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            playerSpawn.playerColorNetVar.Value = NextColor();
        }

            //I feel like there was a better to way to get it to only alter the host, but I couldn't find it and this worked
           /* if (clientId == 0) {
                Player playerSpawn = Instantiate(hostPrefab, NextPosition(), Quaternion.identity);
                playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
                playerSpawn.playerColorNetVar.Value = NextColor(); 
            }
            
            else {
                Player playerSpawn = Instantiate(playerPrefab, NextPosition(), Quaternion.identity);
                playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
                playerSpawn.playerColorNetVar.Value = NextColor();

            } */
            
        
    }
}
