using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
    // Player Prefab to be instantiate
    public GameObject m_PlayerPrefab;
    // List of active player in the game
    public static List<Player> m_PlayerList;

    // The starting camera, also used as spwan point
    private GameObject m_initCamera;
    // Wether local player has joined the game
    private bool m_joined = false;
    
    void Awake() {
        m_PlayerList = new List<Player>();
        m_initCamera = Camera.main.gameObject;
    }

    void OnGUI() {
        // Join button if local player has not yet join the game
        if (!m_joined && GUI.Button(new Rect(10, 10, 200, 40), "Join")) {
            Join();
            m_joined = true;
        }

        // Server disconnected options
        if (!NetLobby.m_isConnect) {
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 40, 200, 40), "Disconnected");
            GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 10, 200, 40), "Press <R> to Restart");

            if (Input.GetKeyDown(KeyCode.R)) {
                // Resume Time scale and clear static params
                Time.timeScale = 1;
                m_PlayerList.Clear();
                // Reload Level
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
        // Server broadcast disconnected player and remove it
        networkView.RPC("RemovePlayer",RPCMode.AllBuffered, player);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info) {
        // Disconnected from server
        // Pause game and resume cursor
        if (GameObject.FindObjectOfType<LockCursor>()) {
            GameObject.FindObjectOfType<LockCursor>().enabled = false;
        }
        Time.timeScale = 0;
        Screen.showCursor = true;
        Screen.lockCursor = false;
    }

    void Join() {
        // Disable the default camera, switch to player view
        m_initCamera.SetActive(false);
        // Initiate player avatar in scene
        networkView.RPC("InitializePlayer", RPCMode.AllBuffered, Network.AllocateViewID(), Network.player);
    }

    // Return the looking player in list
    public static Player FindPlayer(NetworkViewID viewID) {
        foreach(Player player in m_PlayerList){
            if(player.networkView.viewID.Equals(viewID)){
                return player;
            }
        }
        return null;
    }

    // Return the looking player in list
    public static Player FindPlayer(NetworkPlayer playerID) {
        foreach(Player player in m_PlayerList){
            if(player.m_PlayerID.Equals(playerID)){
                return player;
            }
        }
        return null;
    }

    // RPC Call to initialize player across network
    [RPC]
    void InitializePlayer(NetworkViewID viewID, NetworkPlayer playerID, NetworkMessageInfo msgInfo) {
        // Initialize transforms on spwan point
        GameObject playerObj = Instantiate(m_PlayerPrefab) as GameObject;
        playerObj.transform.position = m_initCamera.transform.position;
        playerObj.transform.rotation = m_initCamera.transform.rotation;
        Player player = playerObj.GetComponent<Player>();
        if (player) {
            // Initialize Player network status
            player.Initialize(viewID, playerID);
            // Add player to manage list
            m_PlayerList.Add(player);
        }
    }

    [RPC]
    void RemovePlayer(NetworkPlayer playerID) {
        // Remove the disconnected player from list and destory its gameobject
        Player thisPlayer = FindPlayer(playerID);
        m_PlayerList.Remove(thisPlayer);
        Destroy(thisPlayer.gameObject);
    }
}
