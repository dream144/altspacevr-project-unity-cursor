using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
    public GameObject m_PlayerPrefab;
    public static List<Player> m_PlayerList;

    private bool m_joined = false;

    void Start() {
        m_PlayerList = new List<Player>();

    }

    void OnGUI() {
        if (!m_joined && GUI.Button(new Rect(10, 10, 200, 40), "Join")) {
            Join();
            m_joined = true;
        }
    }

    void Join() {
        // Disable the default camera, switch to player view
        Camera.main.gameObject.SetActive(false);
        // Initiate player avatar in scene
        GetComponent<NetworkView>().RPC("InitializePlayer", RPCMode.AllBuffered, Network.AllocateViewID(), Network.player);
    }

    public static Player FindPlayer(NetworkViewID viewID) {
        foreach(Player player in m_PlayerList){
            if(player.GetComponent<NetworkView>().viewID.Equals(viewID)){
                return player;
            }
        }
        return null;
    }

    public static Player FindPlayer(NetworkPlayer playerID) {
        foreach(Player player in m_PlayerList){
            if(player.m_PlayerID.Equals(playerID)){
                return player;
            }
        }
        return null;
    }

    [RPC]
    void InitializePlayer(NetworkViewID viewID, NetworkPlayer playerID, NetworkMessageInfo msgInfo) {
        GameObject playerObj = Instantiate(m_PlayerPrefab) as GameObject;
        playerObj.transform.position = new Vector3(0, 1, 0);
        Player player = playerObj.GetComponent<Player>();
        if (player) {
            // Initialize Player
            player.Initialize(viewID, playerID);
            m_PlayerList.Add(player);
        }
    }
}
