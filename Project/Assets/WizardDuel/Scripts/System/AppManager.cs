using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppManager : MonoBehaviour {
    public List<GameObject> m_AppPrefabList;

    private Player m_belongPlayer;

	// Use this for initialization1
	void Start () {
        m_belongPlayer = gameObject.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void InstantiateApp(int appIndex) {
        NetApplication netApp = m_AppPrefabList[appIndex].GetComponent<NetApplication>();
        GameObject appObj = Instantiate(m_AppPrefabList[appIndex]) as GameObject;
        netApp = appObj.GetComponent<NetApplication>();
        netApp.Initialize(m_belongPlayer);
    }
    
    void CreateApp(int appIndex) {
        if (appIndex < m_AppPrefabList.Count) {
            NetApplication netApp = m_AppPrefabList[appIndex].GetComponent<NetApplication>();
            if (netApp.m_Local) {
                InstantiateApp(appIndex);
            }
            else {
                GetComponent<NetworkView>().RPC("NetInstantiateApp", RPCMode.AllBuffered, appIndex);
            }
        }
    }

    [RPC]
    void NetInstantiateApp(int appIndex) {
        InstantiateApp(appIndex);
    }

}
