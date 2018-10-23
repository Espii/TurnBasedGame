using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnitPanel : SlotManager {
    public RectTransform rTransform;
    public MouseInputManager myMouseInputManager;
    public UICard CardDisplay;
    public Entity Displayed = null;
    public Game GameInstance;

    void DisplaySelectedCardInfo()
    {
        if (this.MouseOverSlot != null)
        {
            CardDisplay.SetCard((Entity)this.MouseOverSlot.content);
        }
        else
        {
            CardDisplay.SetCard(null);
        }
    }
    

    void NodeSelected(GridNode node)
    {
        Entity e = Global.GetFriendlyEntityOnNode(node);
        Displayed =e;
        PopulatePanel(e);
    }

    void PopulatePanel(Entity e)
    {
        this.ClearSlots();
        if (e==null)
        {
            return;
        }
        if (e.ProductionList.Count>0)
        {
            foreach (Entity pEntity in e.ProductionList)
            {
                Slot slot = this.AddSlot();
                slot.SetContent(pEntity, pEntity.CardImage);
            }
        }
    }

    public override void SelectSlot(Slot slot)
    {
        base.SelectSlot(slot);
        if (slot==null)
        {
            GameInstance.SelectCard(null);
        }
        else
        {
            GameInstance.SelectCard((Entity)slot.content);
        }
    }

    public override void SetMouseOverSlot(Slot slot)
    {
        base.SetMouseOverSlot(slot);
        DisplaySelectedCardInfo();
    }

    protected override void Awake()
    {
        base.Awake();
        myMouseInputManager.AddSelectCallback(NodeSelected);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
