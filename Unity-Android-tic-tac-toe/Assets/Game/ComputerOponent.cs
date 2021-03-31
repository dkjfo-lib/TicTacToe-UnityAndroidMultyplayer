using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface Oponent
{
    string Name { get; }
    bool InTurn { get; }
    bool IsReady { get; }
    TileValue Value { get; }
    Oponent Create(TileValue value);

    void MakeTurn();
}

public enum OponentType
{
    human = 0,
    computer = 1,
    humanLAN = 2
}

public static class OponentFactory
{
    public static Oponent Create(OponentType oponentType)
    {
        Oponent oponent;
        switch (oponentType)
        {
            case OponentType.human:
                oponent = new Player();
                break;
            case OponentType.computer:
                oponent = new ComputerOponent();
                break;
            case OponentType.humanLAN:
                oponent = new OponentLAN();
                break;
            default:
                Debug.LogError("Unknown oponent");
                oponent = null;
                break;
        }
        return oponent;
    }
}

public class ComputerOponent : Oponent
{
    public string Name { get; } = "computer";
    public bool InTurn { get; private set; } = false;
    public bool IsReady { get; private set; } = false;
    public TileValue Value { get; private set; }

    public Oponent Create(TileValue value)
    {
        Value = value;
        IsReady = true;
        return this;
    }

    public void MakeTurn()
    {
        InTurn = true;
        GameGrid.instance.LockSlots();

        var possibleTurns = GetEmptyFields();
        var turn = SelectTile(possibleTurns);
        MakeTurn(turn);

        GameGrid.instance.UnlockEmptySlots();
        InTurn = false;
    }

    Vector2Int[] GetEmptyFields() =>
        GameGrid
            .instance
            .GetEmptySlots()
            .Select(s => new Vector2Int(s.x, s.y))
            .ToArray();

    Vector2Int SelectTile(Vector2Int[] possibleTurns)
    {
        int id = UnityEngine.Random.Range(0, possibleTurns.Length);
        return possibleTurns[id];
    }

    void MakeTurn(Vector2Int tile)
    {
        GameGrid.instance.SetValue(tile, Value);
    }
}

public class Player : Oponent
{
    public static Action OnPlayerSlotChose;

    public string Name { get; } = "player";
    public bool InTurn { get; private set; }
    public bool IsReady { get; private set; } = false;
    public TileValue Value { get; private set; }

    public Oponent Create(TileValue value)
    {
        Value = value;
        OnPlayerSlotChose += () => InTurn = false;
        OnPlayerSlotChose += () => AudioManager.CallAudio("S slot");
        IsReady = true;
        return this;
    }

    public void MakeTurn()
    {
        InTurn = true;
        // sets off on signal from button
    }
}

[System.Serializable]
public class OponentLAN : Oponent
{
    public Action OnPlayerSlotChose;

    public string Name { get; } = "PlayerLAN";
    public bool InTurn { get => inTurn; set => inTurn = value; }
    public bool inTurn;
    public bool IsReady => player != null;
    public TileValue Value { get => value; private set => this.value = value; }
    public TileValue value;
    public PlayerLAN Player { get => this.player; set => this.player = value; }

    public PlayerLAN player;

    public Oponent Create(TileValue value)
    {
        Value = value;
        OnPlayerSlotChose = null;
        OnPlayerSlotChose += () => AudioManager.CallAudio("S slot");
        return this;
    }

    public void MakeTurn()
    {
        InTurn = true;
        // sets off on signal from button
    }
}