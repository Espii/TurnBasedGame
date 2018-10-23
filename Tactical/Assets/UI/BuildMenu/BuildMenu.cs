using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : BasicUI {
    public GameObject BuildButtonPrefab;
    public Transform BuildButtonContainer;
    public CommandMenu command_menu;
    List<BuildButton> ButtonList = new List<BuildButton>();

    void Awake()
    {
        Global.build_menu = this;
        Hide();
    }

    private void Start()
    {
        Global.MouseManager.AddSelectCallback(OnSelect);
    }

    public void OnSelect(GridNode node)
    {
        if (node==null)
        {
            Close();
        }
    }

    public void Close()
    {
        Hide();
        command_menu.command.SetSelectedButton(null);
    }

    public void PopulateMenu(Entity entity)
    {
        foreach (BuildButton bb in ButtonList)
        {
            bb.Die();
        }
        ButtonList.Clear();
        if (entity==null)
        {
            return;
        }
        foreach (Entity e in entity.ProductionList)
        {
            GameObject go=Instantiate(BuildButtonPrefab, BuildButtonContainer);
            BuildButton button=go.GetComponent<BuildButton>();
            ButtonList.Add(button);
            button.menu = this;
            button.SetContent(e);
        }
    }
}
