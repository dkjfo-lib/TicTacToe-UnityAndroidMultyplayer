using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : MonoBehaviour
{
    private const int crossesWinValue = 1;
    private const int circlesWinValue = 8;
    public const int size = 3;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    public static Action OnNewGame;

    public bool multiplayer => NetworkClient.active;
    public Oponent player;
    public Oponent computer;
    public Oponent currentTurnOponent;

    public OponentType oponentType1 = OponentType.human;
    public OponentType oponentType2 = OponentType.computer;

    private Coroutine currentGameLoop;

    public int targetFPS = 30;

    private void Start()
    {
        StartCoroutine(MainGameLoop());
        Application.targetFrameRate = targetFPS;
    }

    IEnumerator MainGameLoop()
    {
        int gameCount = 0;
        while (true)
        {
            Logger.Log($"Game {++gameCount} Started", LogTheme.gameLoop);
            currentGameLoop = StartCoroutine(OneGameLoop());
            yield return currentGameLoop;
        }
    }

    IEnumerator OneGameLoop()
    {
        OnNewGame();
        int turnCount = 0;
        Logger.Log("Starting the game", LogTheme.gameLoop);
        GameGrid.instance.LockSlots();
        yield return StartNewGame(oponentType1, oponentType2);
        if (multiplayer)
        {
            while (multiplayer)
            {
                Server.instance.player1.InitMyPlayer(0);
                Server.instance.player2.InitMyPlayer(1);
                do
                {
                    GameGrid.instance.LockSlots();
                    Logger.Log("Waiting turn", LogTheme.gameDebug);
                    yield return new WaitUntil(() => /*IsGameOver() || */Server.instance.localPlayerTurn);
                    if (IsGameOver())
                    {
                        Logger.Log("Ending mid game", LogTheme.gameDebug);
                        break;
                    }
                    Logger.Log("Starting turn", LogTheme.gameDebug);
                    GameGrid.instance.UnlockEmptySlots();
                    yield return new WaitWhile(() => /*!IsGameOver() ||*/ Server.instance.localPlayerTurn);
                } while (!IsGameOver());
                Logger.Log("game Ended", LogTheme.gameDebug);
                OnNewGame();
            }
        }
        else
        {
            GameGrid.instance.UnlockEmptySlots();
            Logger.Log("Game starting", LogTheme.gameLoop);
            currentTurnOponent = player.Value > computer.Value ? player : computer;
            do
            {
                Logger.Log($"{currentTurnOponent.Name}'s turn, this is {++turnCount} turn of the game", LogTheme.gameLoop);
                currentTurnOponent.MakeTurn();
                yield return new WaitWhile(() => currentTurnOponent.InTurn);
                Logger.Log($"{currentTurnOponent.Name} has made a turn", LogTheme.gameLoop);
                currentTurnOponent = currentTurnOponent == player ? computer : player;
            } while (!IsGameOver());
        }
    }

    public IEnumerator StartNewGame(OponentType type1, OponentType type2)
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        player = OponentFactory.Create(type1).Create((TileValue)rnd + 1);
        computer = OponentFactory.Create(type2).Create((TileValue)2 - rnd);
        yield return new WaitUntil(() => player.IsReady);
        yield return new WaitUntil(() => computer.IsReady);
    }

    public void UpdateAllGameManagers(GameManager newGameManagerState)
    {
        this.player = newGameManagerState.player;
        this.computer = newGameManagerState.computer;
        this.currentTurnOponent = newGameManagerState.currentTurnOponent;
    }

    public bool IsGameOver()
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
            winValue += (int)CheckWinValue(rowValue) + (int)CheckWinValue(columnValue);

            diagonalValue *= (int)slots[(0 + main) * 4].slotValue;
            contrDiagonalValue *= (int)slots[(1 + main) * 2].slotValue;
        }
        winValue += (int)CheckWinValue(diagonalValue) + (int)CheckWinValue(contrDiagonalValue);
        return (TileValue)winValue;
    }

    TileValue CheckWinValue(int value)
    {
        if (value == crossesWinValue) return TileValue.cross;
        if (value == circlesWinValue) return TileValue.circle;
        return TileValue.empty;
    }

    // Called in UI
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