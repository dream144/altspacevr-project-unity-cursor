using UnityEngine;
using System.Collections;

// Update cloth simulation of the sail cloth 
public class SailClothUpdate : MonoBehaviour {
    public ShipSync m_Ship;
    public InteractiveCloth m_Cloth;
    // The force will be applied to the cloth when sail
    public Vector3 m_ClothForce;
    // Colliders which fix the cloth
    public GameObject[] m_fixedPoints;
    
    // Collider initial/full extended positions and rotations
    private Vector3[] m_initPositions;
    private Quaternion[] m_initRotations;
    // Ready for update
    private bool m_ready;

    void Start() {
        m_ready = false;
        m_initPositions = new Vector3[m_fixedPoints.Length];
        m_initRotations = new Quaternion[m_fixedPoints.Length];

        // Record all fully extended positions
        for (int n = 0; n < m_fixedPoints.Length; ++n) {
            m_initPositions[n] = m_fixedPoints[n].transform.localPosition;
            m_initRotations[n] = m_fixedPoints[n].transform.localRotation;
        }

        // Then set all fixed points to starting position of the first collider smoothly
        // Use cooroutine to avoid super bouncy cloth physics
        StartCoroutine(MoveToOringin());
    }

	void Update() {
        // Update Sail cloth extension level based on sail speed
        if (m_ready) {
            float segment = 1.0f / ((float)m_fixedPoints.Length - 1);
            // the minimun index that will go to its fully extended position
            int minIndex = m_fixedPoints.Length;
            for (int n = 1; n < m_fixedPoints.Length; ++n) {
                if (m_Ship.m_SailLevel >= n * segment) {
                    // If ship sail speed is above the thresh, collider go to its fully extended position
                    m_fixedPoints[n].transform.localPosition = m_initPositions[n];
                    m_fixedPoints[n].transform.localRotation = m_initRotations[n];
                }
                else {
                    // Else, collider stick with former collider, but move gradually towards fully extended position 
                    minIndex = minIndex > n ? n : minIndex;
                    float ratio = (m_Ship.m_SailLevel - ((minIndex - 1) * segment)) / segment;
                    // Lerp towards full extended position
                    m_fixedPoints[n].transform.localPosition = Vector3.Lerp(m_initPositions[minIndex - 1], m_initPositions[minIndex], ratio);
                    m_fixedPoints[n].transform.localRotation = Quaternion.Lerp(m_initRotations[minIndex - 1], m_initRotations[minIndex], ratio);
                }
            }

            // Apply force based on sail level to mimic wind
            if (m_Cloth) {
                m_Cloth.externalAcceleration = m_Ship.m_SailLevel * (transform.right * m_ClothForce.x + transform.up * m_ClothForce.y + transform.forward * m_ClothForce.z);
            }
        }
	}

    // Smoothly move all colliders back to the first collider's position
    IEnumerator MoveToOringin() {
        float ratio = 0;
        while (ratio < 1) {
            ratio += Time.fixedDeltaTime / 4;
            ratio = ratio > 1 ? 1 : ratio;
            for (int n = 1; n < m_fixedPoints.Length; ++n) {
                m_fixedPoints[n].transform.localPosition = Vector3.Lerp(m_initPositions[n], m_initPositions[0], ratio);
                m_fixedPoints[n].transform.localRotation = Quaternion.Lerp(m_initRotations[n], m_initRotations[0], ratio);
            }
            yield return new WaitForFixedUpdate();
        }
        m_ready = true;
    }
}
