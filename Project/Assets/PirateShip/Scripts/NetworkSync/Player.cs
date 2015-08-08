using UnityEngine;
using System.Collections;

// Player avatar movement syncrhonization class
public class Player : NetSync {
    // Camera from the avatar view
    public GameObject m_PlayerCamera;
    public NetworkPlayer m_PlayerID;
    public bool m_Movable;

    // The belonged ship script
    private ShipSync m_ship;
    // Whether this avatar is the local avatar that player control
    private bool m_isPlayer;
    // Player input
    private float m_verticalInput;
    private float m_horizontalInput;

	void Awake () {
        // Initialize params
        m_ship = GameObject.FindObjectOfType<ShipSync>();
        m_syncState = new MoveSyncState();
        m_syncState.position = transform.position;
        m_Movable = true;
	}

    public void Initialize(NetworkViewID viewID, NetworkPlayer playerID) {
        // Initialized network states
        networkView.viewID = viewID;
        m_PlayerID = playerID;
        // Check if current avatar is local player and enable player camera
        if (Network.player == playerID) {
            m_isPlayer = true;
            m_PlayerCamera.SetActive(true);
        }
    }

    // Update movement in physics update loop
	void FixedUpdate () {
        if (NetLobby.m_isConnect) {
            MovementUpdate();
        }
	}

    void MovementUpdate() {
        if (m_isPlayer) {
            // Player's own avatar, Assign data to sync states
            if (m_Movable) {
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");

                Vector3 targetDir = transform.forward * verticalInput + transform.right * horizontalInput;
                // Normalize movement
                if (targetDir.magnitude > 1) {
                    targetDir = targetDir.normalized;
                }
                rigidbody.AddForce(targetDir * m_MoveStatus.accerleration);
                
                // Limit avatar relative movement speed compare to the ship
                // To avoid bump warp and extreme movement due to physics
                Vector3 localSpeed = rigidbody.velocity - m_ship.rigidbody.velocity;
                if (localSpeed.magnitude > m_MoveStatus.moveSpeed) {
                    localSpeed = localSpeed * (m_MoveStatus.moveSpeed / localSpeed.magnitude);
                }
                rigidbody.velocity = m_ship.rigidbody.velocity + localSpeed;
            }

            // Broadcast current player move state to others
            // Only perform once per <syncFrequency> frame
            if (m_syncCount == m_NetSyncParams.syncFrequency) {
                m_syncState.position = transform.position;
                m_syncState.velocity = rigidbody.velocity;
                m_syncState.rotation = transform.rotation;
                m_syncCount = 0;
                networkView.RPC("NetMoveSync", RPCMode.Others, m_syncState.position, m_syncState.velocity, m_syncState.rotation);
            }
            else {
                ++m_syncCount;
            }
        }
        else {
            // Interpolation for other player's movement
            // Predict current position from network delay
            float timeDiff = (float)(Network.time - m_syncState.deliverTime);
            Vector3 predictPos = m_syncState.position + m_syncState.velocity * timeDiff;
            Vector3 posDiff = predictPos - transform.position;
            if (posDiff.magnitude < m_NetSyncParams.errorTolerateDistance) {
                // Smooth interpolation towards actual position
                rigidbody.MovePosition(transform.position + m_NetSyncParams.moveSmoothFactor * posDiff);
            }
            else {
                // Instant Wrap to place if position difference is too huge
                rigidbody.MovePosition(predictPos);
            }

            // Perform movement based on sync stats
            rigidbody.velocity = m_syncState.velocity;
            transform.rotation = m_syncState.rotation;
        } 
    }
}
