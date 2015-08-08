using UnityEngine;
using System.Collections;

// Net Synchronization Paramters
[System.Serializable]
public class NetSyncParams {
    // smooth interpolate error factor
    public float moveSmoothFactor;
    // tolerance distance for instant warp calibration
    public float errorTolerateDistance;
    // Synchronization is performce once per <syncFrequency> frame
    public int syncFrequency;
}

// Moving Synchronize paramters
// Pass across network
public class MoveSyncState {
    public Vector3 position;
    public Vector3 velocity;
    public Quaternion rotation;
    public double deliverTime;
}

// Object Move status
[System.Serializable]
public class MoveStatus {
    public float moveSpeed;
    public float accerleration;
    public float rotateSpeed;
}

// Base class of all network synchronize movable objects
public class NetSync : MonoBehaviour {
    public MoveStatus m_MoveStatus;
    public NetSyncParams m_NetSyncParams;
    protected MoveSyncState m_syncState;
    protected int m_syncCount;

    // Sync move data via network
    [RPC]
    protected virtual void NetMoveSync(Vector3 position,
                                       Vector3 velocity,
                                       Quaternion rotation,
                                       NetworkMessageInfo netMsgInfo) {
        m_syncState.position = position;
        m_syncState.velocity = velocity;
        m_syncState.rotation = rotation;
        m_syncState.deliverTime = netMsgInfo.timestamp;
    }

}
