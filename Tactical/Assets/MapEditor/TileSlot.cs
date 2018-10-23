using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class TileSlot : MonoBehaviour, IPointerClickHandler
{
    public Sprite sprite;
    public TileEnum tileEnum;
    public TilePanel panel;
    public Image SelectedBorder;
	public void OnPointerClick(PointerEventData pData)
    {
        if (pData.button==PointerEventData.InputButton.Left)
        {
            panel.SetSelectedSlot(this);
        }
    }

    public void Select()
    {
        SelectedBorder.color = Color.yellow;
    }

    public void Deselect()
    {
        SelectedBorder.color = Color.clear;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (panel.GetSelectedSlot() == this)
            {
                panel.SetSelectedSlot(null);
            }
        }
    }
}
