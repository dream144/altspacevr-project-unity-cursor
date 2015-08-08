using UnityEngine;
using System.Collections;

// Update ship steer wheel rotation smoothly
public class SailWheelRotationUpdate : MonoBehaviour {
    public ShipSync m_Ship;
    public float m_rotateSpeed;

    private float m_currentAngleZ;
	void Start () {
        m_currentAngleZ = transform.localEulerAngles.z;
	}
	
	void Update () {
        // Smoothly rotate the wheel towards the steer angle
        m_currentAngleZ = Mathf.MoveTowards(m_currentAngleZ, -m_Ship.m_SteerDirection * 180, m_rotateSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(0, 0, m_currentAngleZ);
	}
}
