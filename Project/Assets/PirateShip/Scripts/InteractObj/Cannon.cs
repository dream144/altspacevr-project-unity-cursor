using UnityEngine;
using System.Collections;

// Cannon opertion class
public class Cannon : InteractObj{
    // Cannon ball prefab for instanitate
    public GameObject m_CannonBall;
    // Barrel and Barrel muzzle transform
    public GameObject m_Barrel;
    public Transform m_AimingDir;
    public float m_ReloadTime;
    // Player mouse control sensitivity
    public float m_Sensitivity;
    // Horizontal and Vertical angle limit 
    public float m_maxHorizonClamp, m_maxVerticalClamp;

    // Weither the cannon is fireable
    protected bool m_fireable;
    // Initiable rotation state of barrel and muzzle
    private Vector3 m_initEuler;
    private Vector3 m_initBarrelEuler;
    private float m_deltaVertical, m_deltaHorizon;

	void Start () {
        this.meshRenderers = GetComponentsInChildren<MeshRenderer>();
        m_occupied = false;

        m_fireable = true;
        // Record initial aiming direction
        m_initEuler = transform.localEulerAngles;
        m_initBarrelEuler = m_Barrel.transform.localEulerAngles;
	}
	
	protected override void Update () {
        // Get KeyDown Event only works for Update Loop
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            // Only if cannon is used by local player and it is fireable
            if (m_occupied && (m_ctrlPlayer == Network.player) && m_fireable) {
                networkView.RPC("FireSync", RPCMode.All,Network.AllocateViewID(), m_AimingDir.transform.position, m_AimingDir.transform.rotation);
            }
        }
        // Make base.Update running after to make sure no shooting when click select cannon
        base.Update();
	}

    protected override void OnGUI() {
        base.OnGUI();
        // Cannon Operation Instructor
        if (m_occupied && (m_ctrlPlayer == Network.player)) {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 30), m_fireable ? "Ready" : "Reloading");
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height - 60, 300, 30), "Move Mouse to Aim. Left Click to Fire");
        }
    }

    void FixedUpdate() {
        // Update rotation physics in FixedUpdate loop
        if (m_occupied && m_ctrlPlayer == Network.player) {
            // Only able to control if the occupied player is local player
            float horizontal = Input.GetAxis("Mouse X") * m_Sensitivity;
            float vertical = Input.GetAxis("Mouse Y") * m_Sensitivity;

            if (horizontal != 0 || vertical != 0) {
                m_deltaHorizon += horizontal;
                m_deltaVertical += vertical;
                m_deltaHorizon = Mathf.Clamp(m_deltaHorizon, -m_maxHorizonClamp, m_maxHorizonClamp);
                m_deltaVertical = Mathf.Clamp(m_deltaVertical, -m_maxVerticalClamp, m_maxVerticalClamp);
                // Assign new Rotation
                transform.localEulerAngles = m_initEuler + new Vector3(0,m_deltaHorizon,0);
                m_Barrel.transform.localEulerAngles = m_initBarrelEuler + new Vector3(0, 0, m_deltaVertical);
                // Boradcast rotation to other players
                networkView.RPC("BarrelSync", RPCMode.Others, transform.localRotation, m_Barrel.transform.localRotation);
            }
        }
    }

    // Network synchronization for Fire Cannon Ball
    [RPC]
    protected virtual void FireSync(NetworkViewID shellID, Vector3 firePosition, Quaternion fireRotation, NetworkMessageInfo netMsgInfo) {
        // Reset reload timer
        m_fireable = false;
        float delay = (float)(Network.time - netMsgInfo.timestamp);
        StartCoroutine(ReloadTimer(m_ReloadTime - delay));
        // Instaniate Shell Object
        GameObject shell = Instantiate(m_CannonBall) as GameObject;
        shell.networkView.viewID = shellID;
        shell.GetComponent<CannonBallSync>().ShellInitialize(firePosition, fireRotation, netMsgInfo);
    }

    // Netowrk Synchronization for Cannon rotation
    [RPC]
    protected virtual void BarrelSync(Quaternion rotation, Quaternion barrelRotaion) {
        transform.localRotation = rotation;
        m_Barrel.transform.localRotation = barrelRotaion;
    }

    // Reload Counter
    protected virtual IEnumerator ReloadTimer(float time) {
        yield return new WaitForSeconds(time);
        m_fireable = true;
    }
}
