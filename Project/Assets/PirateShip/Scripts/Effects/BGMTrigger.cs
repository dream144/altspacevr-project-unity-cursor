using UnityEngine;
using System.Collections;

// Trigger BGM once the ship start to sail
public class BGMTrigger : MonoBehaviour {
    public ShipSync m_Ship;
    public AudioSource m_BGM;

	// Build Up the interst when SAIL!
	void Update () {
        if (m_Ship.m_SailLevel > 0) {
            if (!m_BGM.isPlaying) {
                m_BGM.Play();
            }
            // Raise volumen based on sail speed
            m_BGM.volume = m_Ship.m_SailLevel;
        }
        else {
            m_BGM.Stop();
        }
	}
}
