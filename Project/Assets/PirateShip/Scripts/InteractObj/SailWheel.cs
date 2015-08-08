using UnityEngine;
using System.Collections;

// Sail steer wheel operation class
public class SailWheel : InteractObj {
    // Belonging ship
    public ShipSync m_ShipCtrl;
    // Texture to be render for GUI
    public Texture2D m_IconTexture;
    // Sensitivity
    public float m_Sensitivity;

    void Start() {
        this.meshRenderers = GetComponentsInChildren<MeshRenderer>();
        m_occupied = false;
    }

    protected override void OnGUI() {
        base.OnGUI();
        // Control Instruction
        if (m_occupied && (m_ctrlPlayer == Network.player)) {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 60, 300, 30), "Turn by A/D. Right Click to Quit");
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 30), "");
            GUI.Box(new Rect(Screen.width / 2 - 10 + m_ShipCtrl.m_SteerDirection * 130, Screen.height - 95, 20, 20), m_IconTexture);
        }
    }

    void FixedUpdate() {
        // Update Ship Sail physics in FixedUpdate loop
        if (m_occupied && m_ctrlPlayer == Network.player) {
            // Only able to control if the occupied player is current player
            float input = Input.GetAxis("Horizontal") * m_Sensitivity;
            float steerDir = m_ShipCtrl.m_SteerDirection;
            if (input != 0) {
                steerDir += input;
                steerDir = Mathf.Clamp(steerDir, -1, 1);
                m_ShipCtrl.SetSteerDir(steerDir);
            }
        }
    }
}
