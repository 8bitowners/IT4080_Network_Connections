using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Arena1Game : NetworkBehaviour
{

    public Player playerPrefab;

    public Player hostPrefab;
    public Camera arenaCamera;
    public GameObject healthPickups;

      private int positionIndex = 0;
      private int hpPositionIndex = 0;
    
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
             SpawnHealthPickups();
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
    }
     private Vector3[] healthPickupPositions = new Vector3[] {
        new Vector3(-60f, 12f, -47f), 
        new Vector3(-45f, 12f, -45f),
        new Vector3(10f, 12f, 50f)
     };
    
    private Vector3 HPPickupNextPosition() {
        Vector3 pos = healthPickupPositions[hpPositionIndex];
        hpPositionIndex += 1;
        if (hpPositionIndex > healthPickupPositions.Length - 1) {
            hpPositionIndex = 0;
        }
        return pos;
    }
    
    private void SpawnHealthPickups() {
            foreach (Vector3 hpSpawnLoc in healthPickupPositions) {
                
                GameObject hpPickup = Instantiate(healthPickups, HPPickupNextPosition(), Quaternion.identity);
                hpPickup.GetComponent<NetworkObject>().Spawn();
            }
        }
}
