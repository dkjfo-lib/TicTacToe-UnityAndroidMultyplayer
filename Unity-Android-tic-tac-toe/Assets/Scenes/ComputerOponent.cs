using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface Oponent
{
    string Name { get; }
    bool InTurn { get; }
    TileValue Value { get; }
    Oponent Create(TileValue value);

    void MakeTurn();
}

public class ComputerOponent : Oponent
{
    public string Name { get; } = "computer";
    public bool InTurn { get; private set; } = false;
    public TileValue Value { get; private set; }

    public Oponent Create(TileValue value)
    {
        Value = value;
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
    public TileValue Value { get; private set; }

    public Oponent Create(TileValue value)
    {
        Value = value;
        OnPlayerSlotChose += () => InTurn = false;
        return this;
    }

    public void MakeTurn()
    {
        InTurn = true;
        // sets off on signal from button
    }
}
