using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerLAN : NetworkBehaviour
{
    public OponentLAN player;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Server.instance.localPlayer = this;
    }

    [ClientRpc]
    public void InitMyPlayer(int id)
    {
        Debug.LogError($"Client: Initing my player {id} {gameObject.name}", gameObject);
        Oponent oponentRole = id == 0 ?
            GameManager.instance.player :
            GameManager.instance.computer;
        player.Create((TileValue)1 + id);
        player = oponentRole as OponentLAN;
        player.inTurn = id == 1;
        player.player = this;
    }

    [Command]
    public void SendTurn2Server(int slotId, PlayerLAN player)
    {
        Debug.Log($"Server: Message from {player}: Slot {slotId} now has value {player.player.Value}", player.gameObject);

        GameGrid.instance.slots[slotId].MakeTurn(player.player.Value, humanThisDevice: false, true);
        if (GameManager.instance.IsGameOver())
            GameGrid.instance.ResetSlots();

        Server.instance.player1.player.inTurn = !Server.instance.player1.player.inTurn;
        Server.instance.player2.player.inTurn = !Server.instance.player2.player.inTurn;

        TileValue[] slots = GameGrid.instance.slots.Select(s => s.slotValue).ToArray();
        SendTurn2Clients(slots, player, Server.instance.player1.player.inTurn);
    }

    [ClientRpc]
    public void SendTurn2Clients(TileValue[] slotValues, PlayerLAN player, bool pl1In)
    {
        Debug.Log($"Client: Message from {player}: Slot {slotValues} now has value {player.player.Value}", player.gameObject);

        Server.instance.player1.player.inTurn = pl1In;
        Server.instance.player2.player.inTurn = !pl1In;

        for (int i = 0; i < GameGrid.instance.slots.Length; i++)
        {
            GameGrid.instance.slots[i].slotValue = slotValues[i];
            GameGrid.instance.slots[i].locked = !Server.instance.localPlayerTurn || slotValues[i] != TileValue.empty;
            GameGrid.instance.slots[i].UpdateSprite2Value();
        }
    }

    [ClientRpc]
    public void StopClient()
    {
        Debug.LogWarning("Client: Disconnecting!");
        
        MyNetworkManager.instance.StopNetwork();

        GameManager.instance.ChangeOponent1(0);
        GameManager.instance.ChangeOponent2(1);
        GameManager.instance.RestartGame();
    }
}
