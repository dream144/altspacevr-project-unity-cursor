using UnityEngine;
using System.Collections;

// Sail speed controller operation class
public class SailRope : InteractObj {
    // Belonging ship
    public ShipSync m_ShipCtrl;
    // Texture to be rendered for GUI
    public Texture2D m_IconTexture;
    // Control sensitivity
    public float m_Sensitivity;

	void Start() {
        this.meshRenderers = GetComponentsInChildren<MeshRenderer>();
        m_occupied = false;
	}

    protected override void OnGUI() {
        base.OnGUI();
        // Control Instruction
        if (m_occupied && (m_ctrlPlayer == Network.player)) {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 60, 300, 30), "Set Sail by W/S. Right Click to Quit");
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 30), "");
            GUI.Box(new Rect(Screen.width / 2 - 140 + m_ShipCtrl.m_SailLevel * 260, Screen.height - 95, 20, 20), m_IconTexture);
        }
    }

    void FixedUpdate() {
        // Update Ship Sail physics in FixedUpdate loop
        if (m_occupied && m_ctrlPlayer == Network.player) { 
            // Only able to control if the occupied player is current player
            float input = Input.GetAxis("Vertical") * m_Sensitivity;
            float sailLevel = m_ShipCtrl.m_SailLevel;
            if(input != 0){
                sailLevel += input;
                sailLevel = Mathf.Clamp(sailLevel, 0, 1);
                m_ShipCtrl.SetSailLevel(sailLevel);
            }
        }
    }

    
}
