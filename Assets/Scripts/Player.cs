using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;    

public class Player : NetworkBehaviour
{

    public NetworkVariable<Color> playerColorNetVar = new NetworkVariable<Color>(Color.blue);
    public NetworkVariable<int> ScoreNetVar = new NetworkVariable<int>(0);
    public BulletSpawner bulletSpawner; 

     public float movementSpeed = 50f;
    public float rotationSpeed = 130f;
    private Camera playerCamera; 
    private GameObject playerBody;

   private void NetworkInit() {
    playerBody = transform.Find("PlayerBody").gameObject;
        
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

         ApplyColor();
         playerColorNetVar.OnValueChanged += OnPlayerColorChanged; 

         if (IsClient) {
            ScoreNetVar.OnValueChanged += ClientOnScoreValueChanged;
         }

   }
   
    private void Awake() {
        NetworkHelper.Log(this, "Awake");
    }

    private void Start() {
        NetworkHelper.Log(this, "Start");
    }

    private void Update() {
        if (IsOwner) {
            OwnerHandleInput();
            if (Input.GetButtonDown("Fire1")) {
                NetworkHelper.Log("Requesting Fire");
                bulletSpawner.FireServerRpc();
            }
        } 
    }

    public override void OnNetworkSpawn() {
        NetworkHelper.Log(this, "OnNetworkSpawn");
        NetworkInit();
        base.OnNetworkSpawn();
    }

    private void ClientOnScoreValueChanged(int old, int current) {
        if (IsOwner) {
            NetworkHelper.Log(this, $"My score is {ScoreNetVar.Value}");
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (IsServer) {
            ServerHandleCollision(collision);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(IsServer) {
            if(other.CompareTag("power_up")) {
                other.GetComponent<BasePowerUp>().ServerPickUp(this);
            }
        }
    }

    private void ServerHandleCollision(Collision collision) {
        
        if(collision.gameObject.CompareTag("bullet")) {
            ulong ownerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            
            NetworkHelper.Log(this, 
                $"Hit by {collision.gameObject.name} " +
                $"owned by {ownerId}");
           
            //The "player other" line isn't working, and keeping it in is actually causing the destroy to not work, so I've commented it out.
            //Presumably if you see this comment I haven't figured out how to fix this.

            Player other = NetworkManager.Singleton.ConnectedClients[ownerId].PlayerObject.GetComponent<Player>();
           
           // This remains commented out not because it didn't work, but because the above line doesn't right now, and I want to minimize potential issues
           // other.ScoreNetVar.Value += 1;
            
            Destroy(collision.gameObject);
            
        }
        
    }

    public void OnPlayerColorChanged(Color previous, Color current) {
        ApplyColor();
    }
   
    private void OwnerHandleInput() {

       Vector3 movement = CalcMovement();
       Vector3 rotation = CalcRotation();

       if(movement != Vector3.zero || rotation != Vector3.zero) {
        MoveServerRpc(CalcMovement(), CalcRotation());
       } 
       
    }

    private void ApplyColor() {
        NetworkHelper.Log(this, $"Applying color {playerColorNetVar.Value}");
        playerBody.GetComponent<MeshRenderer>().material.color = playerColorNetVar.Value;
    }
    [ServerRpc]
    private void MoveServerRpc(Vector3 movement, Vector3 rotation) {
        transform.Translate(movement);
        transform.Rotate(rotation);
    }
    // Rotate around the y axis when shift is not pressed
    
    private Vector3 CalcRotation() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown) {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
        return rotVect;
    }


    // Move up and back, and strafe when shift is pressed
    private Vector3 CalcMovement() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");

        if (isShiftKeyDown) {
            x_move = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed * Time.deltaTime;

       /* Vector3 movementRestrictionCheck = moveVect + transform.position;

            if (movementRestrictionCheck.x >= 5 || movementRestrictionCheck.z >= 5 || movementRestrictionCheck.x <= -5 || movementRestrictionCheck.z <= -5) {
            return Vector3.zero;
        } */
        return moveVect;
    }
}
