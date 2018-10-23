using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*class NodeDisplayInfo
{
    public NodeDisplayInfo(Entity entity, int distance)
    {
        SetEntity(entity, distance);
    }

    public void SetEntity(Entity entity, int distance)
    {
        Entity = entity;
        Distance = distance;
        if (entity == null)
        {
            this.node = null;
        }
        else
        {
            this.node = entity.node;
        }
    }
    public void clear()
    {
        Entity = null;
        Distance = 0;
        this.node = null;
    }
    Entity Entity;
    public GridNode node;
    public int Distance;
}*/


public class MouseInputManager : MonoBehaviour {
    public delegate void NodeSelectDelegate(GridNode node);
    public SelectedUnitPanel sPanel;
    public Vector2Int HighlightedPosition = new Vector2Int(0, 0);
    public UICard CardDisplay;
    //public Grid grid;
    public GridVisual gVisual;
    public CommandMenu cMenu;

    List<NodeSelectDelegate> NodeSelectCallbackList = new List<NodeSelectDelegate>();
    

    public const int PlacementLayer = (int)GridSquareLayers.Placement;

    public void AddSelectCallback(NodeSelectDelegate callback)
    {
        NodeSelectCallbackList.Add(callback);
    }

    void InvokeSelectCallback(GridNode selected)
    {
        for (int i = NodeSelectCallbackList.Count - 1; i >= 0; i--)
        {
            NodeSelectDelegate d=NodeSelectCallbackList[i];
            if (d == null)
            {
                NodeSelectCallbackList.RemoveAt(i);
            }
            else
            {
                d.Invoke(selected);
            }
        }
    }

    public GridNode SelectNode(GridNode node)
    {
        GridNode result=gVisual.SelectNode(node);
        InvokeSelectCallback(result);
        return result;
    }

    public void DeselectNode()
    {
        gVisual.DeselectNode();
        InvokeSelectCallback(null);
        //ClearNodes();
    }

    
    void HandleNodeSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (gVisual.SelectedNode == null)
            {
                SelectNode(gVisual.HighlightedNode);
            }
            else
            {
                if (gVisual.HighlightedNode==null || gVisual.HighlightedNode == gVisual.SelectedNode)
                {
                    return;
                }
                foreach (Entity e in gVisual.HighlightedNode.Occupants)
                {
                    if (e.isOwner())
                    {
                        SelectNode(gVisual.HighlightedNode);
                        return;
                    }
                }
                List<Entity> SelectedOccupants = gVisual.SelectedNode.Occupants;
                Entity friendly = Global.GetFriendlyEntityOnNode(gVisual.SelectedNode);
                if (cMenu.mMode==CommandMenu.MenuMode.Command)
                {
                    if (cMenu.cMode == CommandMenu.ConfirmMode.Move)
                    {
                        friendly.MoveOrAttack(gVisual.HighlightedNode);
                    }
                    else if (cMenu.cMode == CommandMenu.ConfirmMode.Attack)
                    {
                        gVisual.ConfirmAttack(friendly, gVisual.HighlightedNode);
                    }
                }
                else if (cMenu.mMode == CommandMenu.MenuMode.Confirm)
                {
                    if (cMenu.cMode == CommandMenu.ConfirmMode.Attack)
                    {
                        if (gVisual.HighlightedNode == gVisual.ConfirmNode)
                        {
                            cMenu.confirm.Confirm();
                        }
                    }
                }
                /*foreach (Entity entity in SelectedOccupants)
                {
                    if (Global.GameInstance.CurrentTurn && entity.PlayerID == Global.PlayerID)
                    {
                        entity.MoveOrAttack(gVisual.HighlightedNode);
                    }
                }*/
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (cMenu.mMode == CommandMenu.MenuMode.Confirm)
            {
                cMenu.confirm.Cancel();
            }
            else
            {
                DeselectNode();
            }
        }
    }

    public Entity GetEntityOnNode(GridNode node)
    {
        if (node.Occupants.Count > 0)
        {
            return node.Occupants[0];
        }
        return null;
    }

    void HandleGhostPlacement()
    {
        //DeselectNode();
        if (Global.GameInstance.GetSelectedGhost() == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Global.GameInstance.SelectCard(null);
            return;
        }
        GhostEntity ghost = Global.GameInstance.GetSelectedGhost();
        if (gVisual.HighlightedNode != null)
        {
            ghost.SetPosition(gVisual.HighlightedNode);
            if (Input.GetMouseButtonDown(0))
            {
                if (gVisual.GridSquaresContains(gVisual.PlacementGridSquares, gVisual.HighlightedNode))
                {
                    if (gVisual.GetSelectedEntity().ProduceCard(ghost, gVisual.HighlightedNode))
                    {
                        sPanel.SelectSlot(null);
                    }
                }
            }
        }
    }

    public void HandleCardDisplay()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CardDisplay.GetCard()!=null)
            {
                CardDisplay.SetCard(null);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (CardDisplay.GetCard()!=null)
            {
                CardDisplay.SetCard(null);
            }
            else if (gVisual.SelectedNode == null)
            {
                if (gVisual.HighlightedNode != null)
                {
                    CardDisplay.SetCard(GetEntityOnNode(gVisual.HighlightedNode));
                    CardDisplay.SetPosition(Input.mousePosition);
                }
            }
        }
    }
    public GridNode GetSelectedNode()
    {
        return gVisual.SelectedNode;
    }

    private void Awake()
    {
        Global.MouseManager = this;
    }
    // Use this for initialization
    void Start () {
		
	}
    // Update is called once per frame

    void Update() {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
            GridNode node=Global.GameGrid.GetNodeFromPosition(Global.MainCamera.ScreenToWorldPoint(Input.mousePosition));
        gVisual.HighlightNode(node);
        HandleGhostPlacement();
        if (Global.GameInstance.GetSelectedGhost() == null)
        {
            HandleCardDisplay();
            HandleNodeSelect();
        }
        if (gVisual.HighlightedNode != null)
        {
            HighlightedPosition = gVisual.HighlightedNode.GridIndex;
        }
    }
}
