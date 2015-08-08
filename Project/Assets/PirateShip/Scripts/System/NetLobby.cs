using UnityEngine;
using System.Collections;

public class NetLobby : MonoBehaviour {
    // Network connection state
    public static bool m_isConnect = false;
    // Scene Manager Object to be instantiate
    public GameObject m_SceneManagerPrefab;
    public string m_IPAddress = "127.0.0.1";
    public string m_Port = "10800";
    public int m_ServerSize = 32;

    void OnGUI() {
        if (!m_isConnect) {
            // IP and Port input field
            m_IPAddress = GUI.TextField(new Rect(10, 10, 200, 40), m_IPAddress);
            m_Port = GUI.TextField(new Rect(240, 10, 80, 40), m_Port);
            // Host a server
            if (GUI.Button(new Rect(10, 70, 100, 40), "Server")) {
                Network.InitializeServer(m_ServerSize, int.Parse(m_Port), true);
            }
            // Connect as a client
            if (GUI.Button(new Rect(130, 70, 100, 40), "Client")) {
                Network.Connect(m_IPAddress, int.Parse(m_Port));
            }
        }

    }

    
    void OnServerInitialized() {
        // Broadcast Manager Instantiation on initalize the server
        networkView.RPC("CreateManager", RPCMode.AllBuffered, Network.AllocateViewID());
        m_isConnect = true;
    }

    void OnConnectedToServer() {
        m_isConnect = true;
    }

    void OnDisconnectedFromServer(NetworkDisconnection info) {
        NetLobby.m_isConnect = false;
    }

    [RPC]
    void CreateManager(NetworkViewID viewID, NetworkMessageInfo msgInfo) {
        // Instantiate server manager
        GameObject manager = Instantiate(m_SceneManagerPrefab) as GameObject;
        manager.networkView.viewID = viewID;
    }
}
