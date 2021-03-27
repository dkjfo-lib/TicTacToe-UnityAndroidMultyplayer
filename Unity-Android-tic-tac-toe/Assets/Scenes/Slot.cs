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

    private void UpdateSprite2Value() =>
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
            Player.OnPlayerSlotChose();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (locked) return;

        MakeTurn(GameManager.player.Value, humanThisDevice: true, locked: true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (locked) return;

        SetColor(mouseOverColor);
        SetSprite(Value2Sprite(GameManager.player.Value));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (locked) return;

        SetColor(normalColor);
        SetSprite(Value2Sprite(TileValue.empty));
    }
}
