using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TilePanel : MonoBehaviour {
    public Dictionary<TileEnum, Sprite> TileDictionary = new Dictionary<TileEnum, Sprite>();
    TileSlot SelectedSlot;
    public TileSlot GetSelectedSlot()
    {
        return SelectedSlot;
    }
    public void SetSelectedSlot(TileSlot slot)
    {
        if (SelectedSlot!=null)
        {
            SelectedSlot.Deselect();
        }
        if (slot != null)
        {
            slot.Select();
        }
        SelectedSlot=slot;

    }
}
