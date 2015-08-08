using UnityEngine;
using System.Collections;

// Update Player ship sail water splash based on sailing speed
public class WaterSplash : MonoBehaviour {
    public ShipSync m_Ship;
    // Water particles
    public ParticleEmitter[] particles;
    // Max emission states
    private float[] m_initEmissions;
    private float[] m_initEnergy;

	void Start () {
        // record max emission states
        m_initEmissions = new float[particles.Length];
        m_initEnergy = new float[particles.Length];

        for (int n = 0; n < particles.Length; ++n ) {
            m_initEnergy[n] = particles[n].maxEnergy;
            m_initEmissions[n] = particles[n].maxEmission;
        }
	}
	
	// Update particles based on sail speed
	void Update () {
        for (int n = 0; n < particles.Length; ++n) {
            particles[n].minEnergy = m_initEnergy[n] * m_Ship.m_SailLevel / 2;
            particles[n].maxEnergy = m_initEnergy[n] * m_Ship.m_SailLevel;
            particles[n].minEmission = m_initEmissions[n] * m_Ship.m_SailLevel / 2;
            particles[n].maxEmission = m_initEmissions[n] * m_Ship.m_SailLevel;
        }
	}
}
