using UnityEngine;
using System.Collections;

public class DisableSelfRender : MonoBehaviour {
    // Attached on player camera, auto disable the render of the local player mesh
    // Only triggered for the local player's avatar itself
    public GameObject m_Self;

	void OnEnable(){
        // Do not let camera draw the mesh of yourself
        Transform[] objs = m_Self.GetComponentsInChildren<Transform>();
        foreach (Transform obj in objs) {
            obj.gameObject.layer = LayerMask.NameToLayer("PlayerItSelf");
        }
	}
}
