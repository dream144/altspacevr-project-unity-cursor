using UnityEngine;
using System.Collections;

// Cannon ball synchronization class
public class CannonBallSync : NetSync {
    // Fire Muzzle and Explosion prefab on fire
    public GameObject m_FireMuzzle;
    public GameObject m_Explosion;
    // Water splash on hit water
    public GameObject m_WaterSplash;
    // Longest exsit time of cannon ball
    public float m_LifeTime;
    // Possible damage deal to ship
    public float m_Damage;
    // Switch on to calibrate shooting direction for NPC fires 
    public bool m_GravityCalibrate;
	
    public void ShellInitialize(Vector3 firePosition, Quaternion fireRotation, NetworkMessageInfo netMsgInfo) {
        transform.position = firePosition;
        transform.rotation = fireRotation;
        // Calibrate additional velocity base on gravity for NPC Cannon Shell
        if (m_GravityCalibrate) {
            Vector3 fireDir = fireRotation * Vector3.forward;
            Vector3 fireDirXZ = fireDir;
            fireDirXZ.y = 0;
            m_MoveStatus.moveSpeed *= (fireDir.magnitude / fireDirXZ.magnitude);
        }
        // Initialize Speed, add the relative speed of ship
        rigidbody.velocity = transform.forward * m_MoveStatus.moveSpeed + GameObject.FindObjectOfType<ShipSync>().rigidbody.velocity * 0.9f;

        // Do compensation for net sync delay
        float delay = (float)(Network.time - netMsgInfo.timestamp);
        Vector3 preditPosition = firePosition + rigidbody.velocity * delay + (Physics.gravity * delay * delay / 2);
        transform.position = preditPosition;
        // Auto Destrcution after lifetime
        StartCoroutine(LifeTimer(m_LifeTime - delay));
    }

    // Cannon ball does not change its trail at all
    // Thusly once synchronized initally, no further synchronization is required

    // If hit water
    void OnTriggerEnter(Collider cld) {
        if (cld.gameObject.layer.Equals(LayerMask.NameToLayer("Water"))) {
            Instantiate(m_WaterSplash, transform.position, Quaternion.identity);
            Destroy(gameObject,0.3f);
        }
    }

    // Collision with either ship or land
    void OnCollisionEnter(Collision cld) {
        if (cld.transform.root.GetComponent<NPCShip>()) {
            // If hit enemy ship
            Instantiate(m_Explosion, transform.position, Quaternion.identity);
            // Inflict damage on enemy ship
            cld.transform.root.GetComponent<NPCShip>().GetDamage(m_Damage);
            Destroy(gameObject);
        }
        else{
            // If hit terrain
            Instantiate(m_Explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Network broadcast destroy
    [RPC]
    void DestroySync() {
        Destroy(gameObject);
    }

    // Lifetime counter
    IEnumerator LifeTimer(float time) {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
