using UnityEngine;
using System.Collections;

public class NetApplication : MonoBehaviour {
    public Player m_belongPlayer;
    public bool m_Local;

    public virtual void Initialize(Player player) {
        GetComponent<NetworkView>().RPC("NetInitialize", RPCMode.AllBuffered,player.m_PlayerID);
    }

    public virtual void Cancel(Player player) {
        GetComponent<NetworkView>().RPC("NetCancel", RPCMode.AllBuffered, player.m_PlayerID);
    }

    [RPC]
    protected virtual void NetInitialize(NetworkPlayer playerID) {}

    [RPC]
    protected virtual void NetCancel(NetworkPlayer playerID) {}
}
