using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour {
    protected Slot Selected = null;
    protected Slot MouseOverSlot = null;
    public Transform ContentTransform;
    public GameObject SlotPrefab;
    public GridVisual gVisual = null;
    RectTransform PrefabRectTransform;
    public List<Slot> SlotList;
    public float Height;

    protected virtual void Awake()
    {
        PrefabRectTransform = SlotPrefab.GetComponent<RectTransform>();
        Height=PrefabRectTransform.sizeDelta.y;
    }
    public Slot AddSlot()
    {
        GameObject go=Instantiate(SlotPrefab, ContentTransform);
        Slot slot=go.GetComponent<Slot>();
        slot.SetManager(this);
        SlotList.Add(slot);
        return slot;
    }

    public virtual void SelectSlot(Slot slot)
    {
        if (Selected!=null)
        {
            Selected.SetHighlight(false);
        }
        Selected = slot;
        if (slot != null)
        {
            slot.HighlightImage.SetActive(true);
        }
    }

    public void MouseOverEnter(Slot slot)
    {
        SetMouseOverSlot(slot);
    }

    public void MouseOverExit(Slot slot)
    {
        if (MouseOverSlot==slot)
        {
            SetMouseOverSlot(null);
        }
    }

    public virtual void SetMouseOverSlot(Slot slot)
    {
        MouseOverSlot = slot;
    }

    public void ClearSlots()
    {
        for (int i = SlotList.Count - 1; i >= 0; i--)
        {
            SlotList[i].Die();
            SlotList.RemoveAt(i);
        }
    }

}
