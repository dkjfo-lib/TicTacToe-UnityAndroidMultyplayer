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
    public static Oponent currentTurnOponent;

    public OponentType oponentType1 = OponentType.human;
    public OponentType oponentType2 = OponentType.computer;

    private Coroutine currentGameLoop;
    private Coroutine currentGame;

    private void Start()
    {
        currentGame = StartCoroutine(MainGameLoop());
    }

    IEnumerator MainGameLoop()
    {
        int gameCount = 0;
        while (true)
        {
            Debug.LogWarning($"Game {++gameCount} Started");
            currentGameLoop = StartCoroutine(OneGameLoop());
            yield return currentGameLoop;
        }
    }

    IEnumerator OneGameLoop()
    {
        int turnCount = 0;
        StartNewGame(oponentType1, oponentType2);
        currentTurnOponent = player.Value > computer.Value ? player : computer;
        do
        {
            Debug.LogWarning($"{currentTurnOponent.Name}'s turn, this is {++turnCount} turn of the game");
            currentTurnOponent.MakeTurn();
            yield return new WaitWhile(() => currentTurnOponent.InTurn);
            Debug.LogWarning($"{currentTurnOponent.Name} has made a turn");
            currentTurnOponent = currentTurnOponent == player ? computer : player;
        } while (!IsGameOver());
    }

    public void StartNewGame(OponentType type1, OponentType type2)
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        player = OponentFactory.Create(type1).Create((TileValue)rnd + 1);
        computer = OponentFactory.Create(type2).Create((TileValue)2 - rnd);
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

    public void ChangeOponent1(int newType) =>
        oponentType1 = (OponentType)newType;
    public void ChangeOponent2(int newType) =>
        oponentType2 = (OponentType)newType;
    public void RestartGame()
    {
        StopCoroutine(currentGameLoop);
        StartCoroutine(MainGameLoop());
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