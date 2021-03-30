using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public static GameGrid instance;
    private void Awake()
    {
        instance = this;
    }

    public Slot[] slots;

    private void Start()
    {
        GameManager.OnNewGame += ResetSlots;
        slots = GetComponentsInChildren<Slot>();
    }

    public void LockSlots()
    {
        foreach (var slot in slots)
        {
            slot.locked = true;
        }
    }

    public void UnlockEmptySlots()
    {
        foreach (var slot in slots)
        {
            slot.locked = slot.slotValue != TileValue.empty;
        }
    }

    public void ResetSlots()
    {
        foreach (var slot in slots)
        {
            slot.MakeTurn(TileValue.empty, humanThisDevice: false, locked: false);
        }
    }

    public void SetValue(Vector2Int position, TileValue value)
    {
        Slot slot = slots.First(s => s.x == position.x && s.y == position.y);
        slot.MakeTurn(value, humanThisDevice: false, locked: true);
    }

    public Slot[] GetEmptySlots() =>
        slots.Where(s => s.slotValue == TileValue.empty).ToArray();
}
