using UnityEngine;
using System.Collections;

public class NPCCannon : Cannon {
    public float m_FireRange;
    public float m_RandomRange;
    public float m_RandomFireInterval;
    private ShipSync m_playerShip;
	
	void Start () {
        m_occupied = false;
        m_playerShip = null;
        m_fireable = true;
	}
	
	protected override void Update () {
        if (NetLobby.m_isConnect && Network.isServer) {
            if (m_playerShip && m_fireable) {
                // Get to player distance and player ship velocity in XZ plane
                Vector3 playerDistance = m_playerShip.transform.position - transform.position;
                Vector3 playerSpeed = m_playerShip.rigidbody.velocity;
                playerDistance.y = 0;
                playerSpeed.y = 0;
                // Calculate cannon travel time to player ship
                float speedToPlayer = Mathf.Sqrt(Mathf.Pow(m_CannonBall.GetComponent<CannonBallSync>().m_MoveStatus.moveSpeed, 2) - Mathf.Pow(playerSpeed.magnitude, 2));
                float timeReachPlayer = playerDistance.magnitude / speedToPlayer;
                Vector3 predictPlayerPosition = m_playerShip.transform.position + playerSpeed * timeReachPlayer;

                // Get Aimming Direction and compensate gravity drop
                Vector3 aimDistance = predictPlayerPosition - transform.position;
                Vector3 aimDirection = aimDistance - timeReachPlayer * Physics.gravity * 2.25f;

                float playerAngle = Vector3.Angle(aimDistance.normalized, m_Barrel.transform.right);
                if (aimDistance.magnitude < m_FireRange && playerAngle < m_maxHorizonClamp) {
                    // Assign random factor to mimic human error
                    aimDirection += Random.insideUnitSphere * m_RandomRange;
                    networkView.RPC("FireSync", RPCMode.All, Network.AllocateViewID(), m_AimingDir.transform.position, Quaternion.LookRotation(aimDirection.normalized));
                }
            }
        }
	}

    void OnTriggerEnter(Collider cld) {
        if (cld.transform.root.GetComponent<ShipSync>()) {
            m_playerShip = cld.transform.root.GetComponent<ShipSync>();
        }
    }

    void OnTriggerExit(Collider cld) {
        if (cld.transform.root.GetComponent<ShipSync>()) {
            m_playerShip = null;
        }
    }

    protected override IEnumerator ReloadTimer(float time) {
        yield return new WaitForSeconds(time + Random.Range(-m_RandomFireInterval,m_RandomFireInterval));
        m_fireable = true;
    }
}
