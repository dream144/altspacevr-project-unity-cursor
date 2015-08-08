using UnityEngine;
using System.Collections;


// NPC Pirate Ship network synchroinze class
// Currently NPC ship does not move, therefore only sync it's durability state
public class NPCShip : ShipSync{
    // Particle prefabs to be instantiate on destory
    public GameObject m_GiantExplosion;
    public GameObject m_SinkParticle;
    public GameObject m_WheelParent;
    // Durability before sink
    public float m_Durability;
    // Delay of destroy game object after sink
    public float m_DestroyDelay;

    // Get damage from cannon ball
    public void GetDamage(float damage) {
        m_Durability -= damage;
        // Only Sever broadcast damage
        if (Network.isServer) {
            if(m_Durability > 0){
                networkView.RPC("DamageSync", RPCMode.Others, m_Durability);
            }
            else {
                networkView.RPC("SinkSync", RPCMode.All);
            }
        }
    }

    // Broadcast damage across network
    [RPC]
    void DamageSync(float value) {
        m_Durability = value;
    }

    // Broadcast ship sink across network
    [RPC]
    void SinkSync() {
        // Enable and Instantiate sink explosion particles
        m_WheelParent.SetActive(false);
        m_SinkParticle.SetActive(true);
        Instantiate(m_GiantExplosion, m_WheelParent.transform.position, Quaternion.identity);
        // Leave sink particle on water surface
        m_SinkParticle.transform.parent = null;
        // Delay destroy game object
        Destroy(m_SinkParticle, m_DestroyDelay);
        Destroy(gameObject, m_DestroyDelay);
    }
}
