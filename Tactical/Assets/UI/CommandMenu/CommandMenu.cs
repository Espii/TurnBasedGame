using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandMenu : BasicUI
{
    public enum MenuMode {None, Command, Display, Confirm}
    public enum ConfirmMode {None, Move, Attack, Placement }

    public GridVisual gVisual;
    public MenuMode mMode = MenuMode.None;
    public ConfirmMode cMode = ConfirmMode.None;
    public MouseInputManager mInput;
    public CommandPanel command;
    public CommandBackground background;
    public ConfirmPanel confirm;
    public BuildMenu build_menu;

    private void Awake()
    {
        Global.command_menu = this;
    }

    public void ResetMenu()
    {
        OnSelect(gVisual.SelectedNode);
    }

    public override void Hide()
    {
        base.Hide();
        background.gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();
        background.gameObject.SetActive(true);
    }

    public override bool GetDisplayed()
    {
        return (base.GetDisplayed() && background.gameObject.activeSelf);

    }
    public void OnSelect(GridNode node)
    {
        mMode = MenuMode.None;
        cMode = ConfirmMode.None;
        command.Hide();
        confirm.Hide();
        if (node != null)
        {
            if (!command.NodeSelected(node))
            {
                return;
            }
            mMode =MenuMode.Command;
            command.Show();
            transform.position = new Vector3(transform.position.x, Screen.height / 2 + GetHeight()/2, 0);
        } 
    }
    public float GetHeight(BasicUI panel)
    {
        float height = 0;
        if (panel.GetDisplayed())
        {
            CommandButton[] buttons = panel.GetComponentsInChildren<CommandButton>();
            foreach (CommandButton button in buttons)
            {
                if (button.GetDisplayed())
                {
                    height += button.rTransform.sizeDelta.y;
                }
            }
        }
        return height;
    }

    public float GetHeight()
    {
        float height=0;
        height += GetHeight(command);
        height += GetHeight(confirm);
        return height;
    }

    public void GetConfirmation()
    {
        mMode = CommandMenu.MenuMode.Confirm;
        confirm.Show();
        command.Hide();
        transform.position = new Vector3(transform.position.x, Screen.height / 2 + GetHeight()/2, 0);
    }

    // Use this for initialization
    void Start () {
        mInput.AddSelectCallback(OnSelect);
        OnSelect(null);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
