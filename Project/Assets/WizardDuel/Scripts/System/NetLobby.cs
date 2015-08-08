using UnityEngine;
using System.Collections;

public class NetLobby : MonoBehaviour {

    public GameObject m_SceneManagerPrefab;
    public string m_IPAddress = "127.0.0.1";
    public string m_Port = "10800";
    public int m_ServerSize = 32;

    private bool m_isConnect = false;

    void OnGUI() {
        if (!m_isConnect) {
            m_IPAddress = GUI.TextField(new Rect(10, 10, 200, 40), m_IPAddress);
            m_Port = GUI.TextField(new Rect(240, 10, 80, 40), m_Port);

            if (GUI.Button(new Rect(10, 70, 100, 40), "Sever")) {
                Network.InitializeServer(m_ServerSize, int.Parse(m_Port), true);
            }

            if (GUI.Button(new Rect(130, 70, 100, 40), "Client")) {
                Network.Connect(m_IPAddress, int.Parse(m_Port));
            }
        }

    }

    void OnServerInitialized() {
        GetComponent<NetworkView>().RPC("CreateManager", RPCMode.AllBuffered, Network.AllocateViewID());
        m_isConnect = true;
    }

    void OnConnectedToServer() {
        m_isConnect = true;
    }


    [RPC]
    void CreateManager(NetworkViewID viewID, NetworkMessageInfo msgInfo) {
        GameObject manager = Instantiate(m_SceneManagerPrefab) as GameObject;
        manager.GetComponent<NetworkView>().viewID = viewID;
    }
}
