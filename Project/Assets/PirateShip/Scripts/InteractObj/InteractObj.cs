using UnityEngine;
using System.Collections;

// Base Class of all Player-Cursor Interatable Objects
public class InteractObj : Selectable {
    // Name to be display on GUI
    public string m_ObjectName;
    // Distance requried to use the object
    public float m_ActiveRange;

    // Current player which used the object
    protected NetworkPlayer m_ctrlPlayer;
    // Local player
    protected Player m_activePlayer;
    // Whether this object is occupied by a player or not
    protected bool m_occupied;
    

    protected override void Update() {
        base.Update();

        if (NetLobby.m_isConnect) {
            // Find local player
            if (!m_activePlayer) {
                m_activePlayer = SceneManager.FindPlayer(Network.player);
            }

            // Use an object
            if (Input.GetMouseButtonDown(0)) {
                // If local player has select this object and it is not occupied
                if (gameObject == CurrentSelection && !m_occupied) {
                    // If the object is within use distance
                    Vector3 distance = transform.position - m_activePlayer.transform.position;
                    if (distance.magnitude < m_ActiveRange) {
                        // Lock player movement
                        // Assign the object to player, and broadcast the occupation across network
                        m_activePlayer.m_Movable = false;
                        m_activePlayer.GetComponentInChildren<SphericalCursorModule>().LockAndHideCursor();
                        ClaimSailBox(Network.player);
                    }
                }
            }

            // Quit using an object
            if (Input.GetMouseButtonDown(1)) {
                // If local player is the current occupier
                if (m_occupied && m_ctrlPlayer == Network.player) {
                    // Free player from move lock and broadcast release occupation across network
                    m_activePlayer.m_Movable = true;
                    m_activePlayer.GetComponentInChildren<SphericalCursorModule>().ResetCursor();
                    DeclaimSailBox();
                }
            }
        }
    }

    protected virtual void OnGUI() {
        // Object Instruction
        if (gameObject == CurrentSelection) {
            if (m_occupied) {
                if (m_ctrlPlayer != Network.player) {
                    GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 60, 300, 30), m_ObjectName + " - Occupied");
                }
            }
            else {
                Vector3 distance = transform.position - m_activePlayer.transform.position;
                if (distance.magnitude < m_ActiveRange) {
                    GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 60, 300, 30), m_ObjectName + " - Left Click to Use");
                }
                else {
                    GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 60, 300, 30), m_ObjectName + " - Get Closer to Use");
                }
            }
        }
       
    }

    protected void ClaimSailBox(NetworkPlayer playerID) {
        if (!m_occupied) {
            networkView.RPC("ClaimSync", RPCMode.All, playerID);
        }
    }

    protected void DeclaimSailBox() {
        networkView.RPC("DeclaimSync", RPCMode.All);
    }

    // Broadcast object occupation across network
    [RPC]
    protected void ClaimSync(NetworkPlayer player) {
        m_ctrlPlayer = player;
        m_occupied = true;
    }

    // Broacast object release from occupation across network
    [RPC]
    protected void DeclaimSync() {
        m_occupied = false;
    }
}
