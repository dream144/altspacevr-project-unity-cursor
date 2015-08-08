using UnityEngine;
using System.Collections;

public class WizardDuel : NetApplication {
    private enum DuelState {
        WaitForDuel = 0,
        AnnounceDuel = 1,
        OnDuel_Input = 2,
        OnDuel_Judge = 3,
        EndOfDuel = 4
    }

    private Player m_duelistA, m_duelistB;
    private DuelState m_duelState = DuelState.WaitForDuel;

    public void AcceptDuel() {
        GetComponent<NetworkView>().RPC("AcceptDuel", RPCMode.AllBuffered, m_belongPlayer.m_PlayerID);
        m_duelState = DuelState.AnnounceDuel;
    }

    void Update() {
        switch (m_duelState) {
            case DuelState.WaitForDuel:
                if (m_duelistA && m_duelistB) {
                    // If both duelist ready, the host announce the duel
                    if (m_duelistA.m_PlayerID.Equals(Network.player)) {
                        GetComponent<NetworkView>().RPC("AnnounceDuel", RPCMode.AllBuffered);
                    }

                }
                break;

            case DuelState.AnnounceDuel:

                break;

            case DuelState.OnDuel_Input:

                break;

            case DuelState.OnDuel_Judge:

                break;

            case DuelState.EndOfDuel:

                break;
        }
    }


    [RPC]
    protected override void NetInitialize(NetworkPlayer playerID) {
        Player initDuelist = SceneManager.FindPlayer(playerID);
        m_duelistA = initDuelist;
        m_duelistB = null;
        m_duelState = DuelState.WaitForDuel;
    }

    [RPC]
    void AcceptDuel(NetworkPlayer playerID) {
        Player initDuelist = SceneManager.FindPlayer(playerID);
        m_duelistB = initDuelist;
    }

    [RPC]
    void AnnounceDuel() {
        // Disable mobility of both duelist temporary
        m_duelistA.enabled = false;
        m_duelistB.enabled = false;
    } 
}
