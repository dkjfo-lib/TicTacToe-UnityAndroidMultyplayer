using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
     , IPointerClickHandler
     , IPointerEnterHandler
     , IPointerExitHandler
{
    public Sprite emptySprite;
    public Sprite crossSprite;
    public Sprite circleSprite;
    [Space]
    public Color normalColor = Color.white;
    public Color mouseOverColor = new Color(1, 1, 1, .5f);
    [Space]
    public Image displayedValue;
    [Space]
    public bool locked = false;
    public TileValue slotValue;
    public int x;
    public int y;

    private void SetValue(TileValue newValue) =>
        slotValue = newValue;

    private void SetLock(bool locked) =>
        this.locked = locked;

    private void SetSprite(Sprite newSprite) =>
        displayedValue.sprite = newSprite;

    private void SetColor(Color newColor) =>
        displayedValue.color = newColor;

    public void UpdateSprite2Value() =>
        SetSprite(Value2Sprite(slotValue));

    private Sprite Value2Sprite(TileValue value) =>
        value == TileValue.cross ? crossSprite :
        value == TileValue.circle ? circleSprite :
        emptySprite;

    public void MakeTurn(TileValue value, bool humanThisDevice, bool locked)
    {
        SetValue(value);
        UpdateSprite2Value();
        SetColor(normalColor);
        SetLock(locked);

        if (humanThisDevice)
        {
            if (GameManager.instance.multiplayer)
            {
                Server.instance.currentTurnPlayer.player.OnPlayerSlotChose();
                Server.instance.currentTurnPlayer.SendTurn2Server(GameGrid.instance.GetSlotId(this), Server.instance.currentTurnPlayer);
            }
            else
            {
                Player.OnPlayerSlotChose();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (locked) return;

        if (GameManager.instance.multiplayer)
        {
            MakeTurn(Server.instance.currentTurnPlayer.player.Value, humanThisDevice: true, locked: true);
        }
        else
        {
            MakeTurn(GameManager.instance.currentTurnOponent.Value, humanThisDevice: true, locked: true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (locked) return;

        SetColor(mouseOverColor);
        if (GameManager.instance.multiplayer)
        {
            SetSprite(Value2Sprite(Server.instance.currentTurnPlayer.player.Value));
        }
        else
        {
            SetSprite(Value2Sprite(GameManager.instance.currentTurnOponent.Value));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (locked) return;

        SetColor(normalColor);
        SetSprite(Value2Sprite(TileValue.empty));
    }
}
