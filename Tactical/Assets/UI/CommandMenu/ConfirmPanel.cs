using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanel : BasicUI {
    public CommandMenu menu;
    public bool Hidden = false;

    public void Confirm()
    {
        Global.GetFriendlyEntityOnNode(menu.gVisual.SelectedNode).MoveOrAttack(menu.gVisual.ConfirmNode);
        menu.mInput.SelectNode(null);
    }

    public void Cancel()
    {
        menu.mInput.SelectNode(menu.gVisual.SelectedNode);
    }
}
