using UnityEngine;
using System.Collections;

// Update NPC pirate ship on fire state
public class ShipOnFire : MonoBehaviour {
    public NPCShip m_NPCShip;
    // Positions that could be on fire
    public GameObject[] m_FireSpots;
    // Initial durability of the NPC ship
    private float m_initDurability;
    // Last actived fire spot index
    private int m_lastIndex;

	void Start () {
        m_initDurability = m_NPCShip.m_Durability;
        m_lastIndex = 0;
	}
	
	void Update () {
        if (m_NPCShip.m_Durability > 0) {
            // Enable fire based on ship health
            float segment = m_initDurability / (m_FireSpots.Length + 1);
            for (int n = m_lastIndex; n < m_FireSpots.Length; ++n) {
                if ((m_initDurability - m_NPCShip.m_Durability) >= (n + 1) * segment) {
                    m_FireSpots[n].SetActive(true);
                    m_lastIndex = n;
                }
            }
        }
        else {
            // On Ship Sink
            // Stop emit but not disappear suddenly
            for (int n = m_lastIndex; n < m_FireSpots.Length; ++n) {
                ParticleEmitter[] emitters = m_FireSpots[n].GetComponentsInChildren<ParticleEmitter>();
                foreach (ParticleEmitter emitter in emitters) {
                    emitter.emit = false;
                }
                
            }
            this.enabled = false;
        }
	}
}
