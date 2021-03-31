using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Server : NetworkBehaviour
{
    public static Server instance;
    private void Awake()
    {
        instance = this;
    }

    public PlayerLAN localPlayer;
    public PlayerLAN player1;
    public PlayerLAN player2;
    public PlayerLAN currentTurnPlayer => player1.player.inTurn ? player1 : player2;
    public bool localPlayerTurn => localPlayer.player.inTurn;

    [Command]
    public void UpdateServer(PlayerLAN pl1, PlayerLAN pl2)
    {
        Debug.LogError("Server: update o players");
        player1 = pl1;
        player2 = pl2;
    }

    [ClientRpc]
    public void SendUpdatePl1(PlayerLAN pl1)
    {
        Debug.Log($"Client: New pl1 value from server {pl1?.gameObject.name}", pl1);
        player1 = pl1;
    }

    [ClientRpc]
    public void SendUpdatePl2(PlayerLAN pl2)
    {
        Debug.Log($"Client: New pl2 value from server {pl2?.gameObject.name}", pl2);
        player2 = pl2;
    }
}
