using UnityEngine;
using System.Collections;

// Player controlled ship synchronization class
public class ShipSync : NetSync {
    // Car(Wheel) physics object
    public WheelCollider m_FrontLeft, m_FrontRight, m_RearLeft, m_RearRight;
    // Sail Speed
    public float m_SailLevel;
    // Steer Direction
    public float m_SteerDirection;

    void Awake() {
        m_syncCount = 0;
        m_syncState = new MoveSyncState();
        m_syncState.position = transform.position;
    }

    void FixedUpdate() {
        if (NetLobby.m_isConnect) {
            MovementUpdate();
        }
    }

    void MovementUpdate() {
        // Update Ship(Car) Physics
        m_FrontRight.motorTorque = m_SailLevel * m_MoveStatus.accerleration;
        m_FrontLeft.motorTorque = m_SailLevel * m_MoveStatus.accerleration;
        m_RearLeft.steerAngle = -m_SteerDirection * m_MoveStatus.rotateSpeed;
        m_RearRight.steerAngle = -m_SteerDirection * m_MoveStatus.rotateSpeed;

        if (Network.isServer) {
            // Sever Broadcast ship movement
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
            // Interpolation from server's ship movement
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
            // Synchronize postion and velocity based on sync stats
            rigidbody.velocity = m_syncState.velocity;
            transform.rotation = m_syncState.rotation;
        }
    }

    public void SetSailLevel(float sailLevel) {
        if (sailLevel != m_SailLevel) {
            // Only boradcast if there is difference
            networkView.RPC("UpdateSail", RPCMode.All, sailLevel);
        }
    }

    public void SetSteerDir(float steerDir) {
        if (steerDir != m_SteerDirection) {
            // Only boradcast if there is difference
            networkView.RPC("UpdateSteer", RPCMode.All, steerDir);
        }
    }

    void OnPlayerConnected(NetworkPlayer player) { 
        // Broadcast Sail and Steer state to new player
        networkView.RPC("UpdateSail", player, m_SailLevel);
        networkView.RPC("UpdateSteer", player, m_SteerDirection);
    }

    // Sync sail level across network
    [RPC]
    void UpdateSail(float sailLevel) {
        m_SailLevel = sailLevel;
    }

    // Sync steer direction across network
    [RPC]
    void UpdateSteer(float steerDir) {
        m_SteerDirection = steerDir;
    }
}
