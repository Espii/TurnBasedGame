using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPanel : BasicUI
{
    public CommandMenu menu;
    public CommandButton MoveButton;
    public CommandButton AttackButton;
    public CommandButton BuildButton;
    public CommandButton HealButton;
    public CommandButton CancelButton;
    public GridVisual gVisual;
    public Color SelectedColor = Color.yellow;
    private Entity DisplayedUnit;
    private CommandButton SelectedButton = null;

    public void SetDisplayedUnit(Entity unit)
    {
        DisplayedUnit = unit;
        if (unit == null)
        {
            MoveButton.UpdateButton(false);
            AttackButton.UpdateButton(false);
            BuildButton.UpdateButton(false);
            HealButton.UpdateButton(false);
            return;
        }
        if (unit.TotalMovement>0)
            MoveButton.UpdateButton(true);
        else
            MoveButton.UpdateButton(false);

        if (unit.Attack > 0)
            AttackButton.UpdateButton(true);
        else
            AttackButton.UpdateButton(false);

        if (unit.ProductionList.Count > 0)
            BuildButton.UpdateButton(true);
        else
            BuildButton.UpdateButton(false);

        if (unit.Heal > 0)
            HealButton.UpdateButton(true);
        else
            HealButton.UpdateButton(false);
    }

    void ButtonPress(CommandButton button)
    {
        gVisual.ClearNodes(GridSquareLayers.Select);
        menu.build_menu.Close();
        SetSelectedButton(button);
    }

    public void SetSelectedButton(CommandButton button)
    {
        if (SelectedButton != null)
            SelectedButton.Deselect();
        if (button == null)
            return;
        button.Select(SelectedColor);
        SelectedButton = button;
    }
    public void Move()
    {
        ButtonPress(MoveButton);
        menu.cMode = CommandMenu.ConfirmMode.Move;
        gVisual.DisplayMovement(DisplayedUnit);
        
    }
    public void Attack()
    {

        ButtonPress(AttackButton);
        menu.cMode = CommandMenu.ConfirmMode.Attack;
        gVisual.DisplayAttack(DisplayedUnit);
    }

    public void Heal()
    {
        SetSelectedButton(HealButton);
    }

    public void Build()
    {
        ButtonPress(BuildButton);
        //gVisual.DisplayPlacement(DisplayedUnit);
        menu.build_menu.PopulateMenu(Global.GetFriendlyEntityOnNode(gVisual.SelectedNode));
        menu.cMode = CommandMenu.ConfirmMode.Placement;
        menu.build_menu.Show();
        //Hide();
    }

    public void Cancel()
    {
        ButtonPress(null);
        Global.MouseManager.DeselectNode();
        SetDisplayedUnit(null);
        Hide();
    }
    public bool NodeSelected(GridNode node)
    {
        if (Global.GetEnemyEntityOnNode(node)!=null)
        {
            return false;
        }
        Entity e = Global.GetFriendlyEntityOnNode(node);
        if (!e.GetAction())
        {
            return false;
        }
        SetDisplayedUnit(e);
        return true;
    }

    // Use this for initialization
    void Start () {
        SetDisplayedUnit(null);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
