using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager instance;
    public override void Awake()
    {
        base.Awake();
        instance = this;
    }

    List<PlayerLAN> players;

    public override void OnStartServer()
    {
        base.OnStartServer();
        players = new List<PlayerLAN>();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        Server server = Server.instance;

        var playerLAN = player.GetComponent<PlayerLAN>();
        int id = server.player1 == null ? 0 : 1;

        playerLAN.player.player = playerLAN;
        playerLAN.player.inTurn = id == 1;
        playerLAN.player.Create((TileValue)1 + id);
        if (id == 0)
        {
            GameManager.instance.player.Create(TileValue.cross);
            (GameManager.instance.player as OponentLAN).player = playerLAN;
            server.player1 = playerLAN;
        }
        else
        {
            GameManager.instance.computer.Create(TileValue.circle);
            (GameManager.instance.computer as OponentLAN).player = playerLAN;
            server.player2 = playerLAN;
        }
        players.Add(playerLAN);

        server.SendUpdatePl1(server.player1);
        server.SendUpdatePl2(server.player2);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].InitMyPlayer(i);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.LogError("Client: oponent disconnected!");
        StopNetwork();
        GameManager.instance.ChangeOponent1(0);
        GameManager.instance.ChangeOponent2(1);
        GameManager.instance.RestartGame();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.LogWarning("Server: Disconnect Clients!");
        for (int i = 0; i < players.Count; i++)
        {
            players[i]?.StopClient();
        }
        StopNetwork();
    }

    public void StopNetwork()
    {
        Server.instance.player1 = null;
        Server.instance.player2 = null;
        Server.instance.localPlayer = null;

        if (NetworkClient.active)
        {
            StopClient();
        }
        if (NetworkServer.active)
        {
            StopServer();
        }
    }
}
