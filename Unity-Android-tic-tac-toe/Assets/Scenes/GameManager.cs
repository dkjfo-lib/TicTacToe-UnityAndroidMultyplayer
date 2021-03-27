using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int crossesWinValue = 1;
    private const int circlesWinValue = 8;
    public const int size = 3;

    public static Action OnNewGame;

    public static Oponent player;
    public static Oponent computer;

    private void Start()
    {
        StartCoroutine(MainGameLoop());
    }

    IEnumerator MainGameLoop()
    {
        int gameCount = 0;
        while (true)
        {
            Debug.LogWarning($"Game {++gameCount} Started");
            yield return StartCoroutine(OneGameLoop());
        }
    }

    IEnumerator OneGameLoop()
    {
        int turnCount = 0;
        StartNewGame();
        Oponent oponent = player.Value > computer.Value ? player : computer;
        do
        {
            Debug.LogWarning($"{oponent.Name}'s turn, this is {++turnCount} turn of the game");
            oponent.MakeTurn();
            yield return new WaitWhile(() => oponent.InTurn);
            Debug.LogWarning($"{oponent.Name} has made a turn");
            oponent = oponent == player ? computer : player;
        } while (!IsGameOver());
        StartNewGame();
    }

    public void StartNewGame()
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        player = new Player().Create((TileValue)rnd + 1);
        computer = new ComputerOponent().Create((TileValue)2 - rnd);
        OnNewGame();
    }

    bool IsGameOver()
    {
        TileValue winner = WhoWon();
        bool isMapFull = GameGrid.instance.GetEmptySlots().Length == 0;
        return winner != TileValue.empty || isMapFull;
    }

    TileValue WhoWon()
    {
        var slots = GameGrid.instance.slots;

        int winValue = 0;
        int diagonalValue = 1;
        int contrDiagonalValue = 1;
        for (int main = 0; main < size; main++)
        {
            int rowValue = 1;
            int columnValue = 1;
            for (int sub = 0; sub < size; sub++)
            {
                rowValue *= (int)slots[main + sub * 3].slotValue;
                columnValue *= (int)slots[sub + main * 3].slotValue;
            }
            winValue += (int)CheckValue(rowValue) + (int)CheckValue(columnValue);

            diagonalValue *= (int)slots[(0 + main) * 4].slotValue;
            contrDiagonalValue *= (int)slots[(1 + main) * 2].slotValue;
        }
        winValue += (int)CheckValue(diagonalValue) + (int)CheckValue(contrDiagonalValue);
        return (TileValue)winValue;
    }

    TileValue CheckValue(int value)
    {
        if (value == crossesWinValue) return TileValue.cross;
        if (value == circlesWinValue) return TileValue.circle;
        return TileValue.empty;
    }
}


// game of tic tac toe has 3 values
// cross circle and empty
public enum TileValue
{
    empty = 0,
    cross = 1,
    circle = 2
}

public struct TurnData
{
    public int X { get; set; }
    public int Y { get; set; }
    public TileValue Value { get; set; }
    public bool IsPlayer { get; set; }
}