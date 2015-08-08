using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    [System.Serializable]
    public class MoveStatus {
        public float moveSpeed;
        public float accerleration;
        public float rotateSpeed;
    }

    [System.Serializable]
    public class NetSyncParams {
        public float moveSmoothFactor;
        public float errorTolerateDistance;
        public int syncFrequency;
    }

    private class MoveSyncState {
		public Vector3 position;
		public Vector3 velocity;
		public Quaternion rotation;
        public double deliverTime;
	}
    
    public GameObject m_PlayerCamera;
    public NetworkPlayer m_PlayerID;
    public MoveStatus m_MoveStatus;
    public NetSyncParams m_NetSyncParams;
    
    private MoveSyncState m_syncState;
    private bool m_isPlayer;
    private float m_verticalInput;
    private float m_horizontalInput;

    private int m_syncCount;


	// Use this for initialization
	void Awake () {
        m_syncState = new MoveSyncState();
        m_syncState.position = transform.position;
	}

    public void Initialize(NetworkViewID viewID, NetworkPlayer playerID) {
        GetComponent<NetworkView>().viewID = viewID;
        m_PlayerID = playerID;
        if (Network.player == playerID) {
            m_isPlayer = true;
            m_PlayerCamera.SetActive(true);
        }
    }

    void Update() {
        // Input events are based on Update() Loop
        // Thus update input params in Update()
        m_verticalInput = Input.GetAxis("Vertical");
        m_horizontalInput = Input.GetAxis("Horizontal");
    }

	// Update is called once per frame
	void FixedUpdate () {
        MovementUpdate();
	}

    protected virtual void MovementUpdate() {
        if (m_isPlayer) {
            // Player's own avatar, Assign data to sync states
            Vector3 targetDir = transform.forward * m_verticalInput + transform.right * m_horizontalInput;
            if(targetDir.magnitude > 1){
                targetDir = targetDir.normalized;
            }

            m_syncState.position = transform.position;
            m_syncState.velocity = Vector3.MoveTowards(GetComponent<Rigidbody>().velocity, targetDir * m_MoveStatus.moveSpeed, m_MoveStatus.accerleration);
            m_syncState.rotation = transform.rotation;

            // Broadcast current player move state to others
            if (m_syncCount == m_NetSyncParams.syncFrequency) {
                m_syncCount = 0;
                GetComponent<NetworkView>().RPC("NetMoveSync", RPCMode.Others, m_syncState.position, m_syncState.velocity, m_syncState.rotation);
            }
            else {
                ++m_syncCount;
            }
        }
        else {
            // Interpolation for other player's movement
            float timeDiff = (float)(Network.time - m_syncState.deliverTime);
            Vector3 predictPos = m_syncState.position + m_syncState.velocity * timeDiff;
            Vector3 posDiff = predictPos - transform.position;
            if (posDiff.magnitude < m_NetSyncParams.errorTolerateDistance) {
                // Smooth interpolation towards actual position
                m_syncState.velocity = (m_syncState.velocity.normalized * (1.0f - m_NetSyncParams.moveSmoothFactor) + posDiff.normalized * m_NetSyncParams.moveSmoothFactor) * m_syncState.velocity.magnitude;
            }
            else {
                // Instant Wrap to place if position difference is too huge
                transform.position = predictPos;
            }
        }
        
        // Perform movement based on sync stats
        GetComponent<Rigidbody>().velocity = m_syncState.velocity;
        transform.rotation = m_syncState.rotation;
    }

    // Sync move data via network data
    [RPC]
    void NetMoveSync(Vector3 position,
                     Vector3 velocity,
                     Quaternion rotation,
                     NetworkMessageInfo netMsgInfo) {
        m_syncState.position = position;
        m_syncState.velocity = velocity;
        m_syncState.rotation = rotation;
        m_syncState.deliverTime = netMsgInfo.timestamp;
    }
}
