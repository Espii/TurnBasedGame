using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour {
    public SelectedUnitPanel sPanel;
    public CommandMenu cMenu;

    public GridNode HighlightedNode = null;
    public GridNode SelectedNode = null;
    public GridNode ConfirmNode = null;
    public Grid grid;

    public Color HighlightColor = Color.gray;
    public Color PlacementColor = Color.white;
    public Color SelectColor = Color.yellow;
    public Color AttackColor = Color.red;
    public Color MovementColor = Color.green;
    public Color DefaultColor = Color.clear;

    //public List<DistanceFromNodeResult> HighlightDisplayNodes = new List<DistanceFromNodeResult>();
    //public List<DistanceFromNodeResult> SelectedDisplayNodes = new List<DistanceFromNodeResult>();
    //public List<DistanceFromNodeResult> PlacementDisplayNodes = new List<DistanceFromNodeResult>();

    public List<GridSquare> SelectGridSquares = new List<GridSquare>();
    public List<GridSquare> HighlightGridSquares = new List<GridSquare>();
    public List<GridSquare> PlacementGridSquares = new List<GridSquare>();
    //public List<GridSquare> MouseInputGridSquares = new List<GridSquare>();

    public bool GridSquaresContains(List<GridSquare> list, GridNode node)
    {
        foreach (GridSquare gs in list)
        {
            if (gs.node==node)
            {
                return true;
            }
        }
        return false;
    }

    GridSquare SelectedSquare;

    public Dictionary<int, List<GridSquare>> LookupList = new Dictionary<int, List<GridSquare>>();

    public const int HighlightLayer = (int)GridSquareLayers.Highlight;
    public const int SelectLayer = (int)GridSquareLayers.Select;
    public const int PlacementLayer = (int)GridSquareLayers.Placement;
    //public const int MouseInputLayer = (int)GridSquareLayers.MouseInput;

    MouseInputManager mouse_manager = null;

    private void Awake()
    {
        LookupList[HighlightLayer] = HighlightGridSquares;
        LookupList[SelectLayer] = SelectGridSquares;
        LookupList[PlacementLayer] = PlacementGridSquares;
        //LookupList[MouseInputLayer] = MouseInputGridSquares;
    }
    void Start()
    {
        mouse_manager=Global.MouseManager;
    }

    public void ClearNodes(GridSquareLayers layer)
    {
        /*List<DistanceFromNodeResult> NodeList = null;
        if (layer==GridSquareLayers.Highlight)
        {
            NodeList = HighlightDisplayNodes;
        }
        else if (layer==GridSquareLayers.Select)
        {
            NodeList = SelectedDisplayNodes;
        }
        else if (layer==GridSquareLayers.Placement)
        {
            NodeList = PlacementDisplayNodes;
        }
        if (NodeList==null)
        {
            return;
        }
        foreach (DistanceFromNodeResult res in NodeList)
        {
             res.node.GridSquares[(int)layer].sprite_renderer.color = DefaultColor;
        }*/
        foreach(GridSquare gs in LookupList[(int)layer])
        {
            gs.Die();
        }
        LookupList[(int)layer].Clear();
        
        //NodeList.Clear();
    }

    /*void ClearNodes(List<DistanceFromNodeResult> nodes, int Layer)
    {*/
        /*foreach (DistanceFromNodeResult n in nodes)
        {
            n.node.GridSquares[Layer].GetComponent<SpriteRenderer>().color = DefaultColor;
        }*/
        /*foreach (GridSquare gs in LookupList[Layer])
        {
            gs.Die();
        }
        LookupList[Layer].Clear();
        nodes.Clear();
    }*/


    List<DistanceFromNodeResult> RemoveUnwalkableNodes(Entity entity, List<DistanceFromNodeResult> nodes)
    {
        for (int i=nodes.Count-1; i>=0; i--)
        {
            DistanceFromNodeResult result = nodes[i];
            if (!entity.isWalkable(result.node))
            {
                nodes.RemoveAt(i);
            }
        }
        return nodes;
    }

    public bool DisplayMovement()
    {
        Entity e = Global.GetFriendlyEntityOnNode(SelectedNode);
        if (e != null)
        {
            return DisplayMovement(e);
        }
        return false;
    }


    public bool DisplayMovement(Entity entity)
    {
        if (!entity.IsOwner() || !Global.GameInstance.CurrentTurn)
            return false;
        /*if (!entity.CanMove())
            return false;*/
        ClearNodes(GridSquareLayers.Select);
        List<DistanceFromNodeResult> nodes = Global.GameGrid.GetNodesBFSDistanceFromNode(entity, entity.GetNode(), entity.Movement);
        //RemoveUnwalkableNodes(entity, nodes);
        foreach (DistanceFromNodeResult n in nodes)
        {
            //n.node.GridSquares[SelectLayer].sprite_renderer.color = MovementColor;)
            if (!entity.isWalkable(n.node))
            {
                Entity enemy = Global.GetEnemyEntityOnNode(n.node);
                if (enemy!=null)
                {
                    continue;
                }
            }
            else
            {
                if (n.distance <= entity.Movement)
                {
                    SelectGridSquares.Add(grid.CreateGridSquare(n.node, SelectLayer, MovementColor));
                }
            }

        }
        return true;
        //SelectedDisplayNodes = nodes;
    }

    public void ConfirmMovement(GridNode node)
    {
        if (!ConfirmFunc(node))
        {
            return;
        }
        SelectGridSquares.Add(grid.CreateGridSquare(node, SelectLayer, MovementColor));
    }

    public bool DisplayAttack(Entity entity)
    {
        if (!entity.CanAttack() && entity.isOwner())
            return false;
        int Distance = entity.Range;
        List<DistanceFromNodeResult> nodes = null;
        if (!entity.isOwner())
        {
            Distance = entity.Range + entity.TotalMovement;
        }
        nodes = Global.GameGrid.GetNodesBFSDistanceFromNode(entity.GetNode(), Distance);
        GridSquareLayers layer = GridSquareLayers.Highlight;
        List<GridSquare> list = null;
        if (entity.GetNode() == SelectedNode)
        {
            list = SelectGridSquares;
            layer = GridSquareLayers.Select;
            //ClearNodes(layer);
            //SelectedDisplayNodes.AddRange(nodes);
        }
        else if (entity.GetNode() == HighlightedNode)
        {
            list = HighlightGridSquares;
            layer = GridSquareLayers.Highlight;
            //ClearNodes(layer);
            //HighlightDisplayNodes.AddRange(nodes);
        }
        
        if (list==null)
        {
            return false;
        }
        foreach (DistanceFromNodeResult n in nodes)
        {
            if (!entity.isWalkable(n.node))
            {
                if (n.node.tile_enum == TileEnum.empty)
                {
                    continue;
                }
            }
            //n.node.GridSquares[(int)layer].sprite_renderer.color = AttackColor;
            //int order = n.node.GetOrder() + GridNode.HighlightLayer;
            
            int order = GridNode.HighlightLayer;
            if (layer == GridSquareLayers.Select)
            {
                //order = n.node.GetOrder() + GridNode.SelectLayer;
                order = GridNode.SelectLayer;
            }   
            list.Add(grid.CreateGridSquare(n.node, order, AttackColor));
            
        }
        return true;
    }

    bool ConfirmFunc(GridNode node)
    {
        if (!GridSquaresContains(SelectGridSquares, node))
        {
            return false;
        }
        ClearNodes(GridSquareLayers.Select);
        ClearNodes(GridSquareLayers.Highlight);
        ConfirmNode = node;
        cMenu.GetConfirmation();
        return true;
    }
    public void ConfirmAttack(Entity entity, GridNode node)
    {
        if (Global.GetEnemyEntityOnNode(node) == null)
        {
            return;
        }
        if (!ConfirmFunc(node))
        {
            return;
        }
        List<DistanceFromNodeResult> nodes=Global.GameGrid.GetNodesBFSDistanceFromNode(node, entity.AreaAttackRange);
        foreach (DistanceFromNodeResult n in nodes)
        {
            SelectGridSquares.Add(grid.CreateGridSquare(n.node, SelectLayer, AttackColor));
        }
    }

    public Entity GetSelectedEntity()
    {
        return Global.GetEntityOnNode(SelectedNode);
    }
    public void DisplayPlacement(Entity displayed)
    {
        ClearNodes(GridSquareLayers.Select);
        ClearNodes(GridSquareLayers.Highlight);
        ClearNodes(GridSquareLayers.Placement);
        if (displayed==null)
        {
            return;
        }
        if (displayed.GetAction())
        {
            List<DistanceFromNodeResult> nodes = Global.GameGrid.GetNodesBFSDistanceFromNode(displayed.GetNode(), displayed.ProductionRange);
            foreach (DistanceFromNodeResult result in nodes)
            {
                GhostEntity ghost = Global.GameInstance.GetSelectedGhost();
                GridNode n=result.node;
                if (ghost!=null && ghost.card.entity_type == Entity.EntityType.Resource)
                {
                    if (!ghost.card.isWalkable(n))
                    {
                        continue;
                    }
                }
                //result.node.GridSquares[PlacementLayer].sprite_renderer.color = PlacementColor;
                //int order = result.node.GetOrder() + GridNode.PlacementLayer;
                int order = GridNode.PlacementLayer;
                PlacementGridSquares.Add(grid.CreateGridSquare(result.node, order, PlacementColor));
            }
            //PlacementDisplayNodes.AddRange(nodes);
        }
    }

    public void HighlightNode(GridNode node)
    {
        if (HighlightedNode != node)
        {
            ClearNodes(GridSquareLayers.Highlight);
            if (HighlightedNode != SelectedNode)
            {
                /*if (HighlightedNode != null)
                {
                    HighlightedNode.GridSquares[MouseInputLayer].GetComponent<SpriteRenderer>().color = DefaultColor;
                }*/
            }
            if (node != null)
            {
                if (node != SelectedNode)
                {
                    if (ConfirmNode == node && cMenu.mMode == CommandMenu.MenuMode.Confirm)
                    {

                    }
                    else
                    {
                        //node.GridSquares[MouseInputLayer].GetComponent<SpriteRenderer>().color = HighlightColor;)
                        HighlightGridSquares.Add(grid.CreateGridSquare(node, HighlightLayer, HighlightColor));
                    }
                }
            }
            HighlightedNode = node;
            if (node != null && SelectedNode==null)
            {
                foreach (Entity e in node.Occupants)
                {
                    if (!e.isOwner())
                        DisplayAttack(e);
                }
            }
        }
    }

    public GridNode SelectNode(GridNode node)
    {
        DeselectNode();
        GridNode result = null;
        if (node != null)
        {
            if (node.Occupants.Count > 0)
            {
                SelectedNode = node;
                int order = GridNode.MouseInput;
                SelectedSquare =grid.CreateGridSquare(SelectedNode, order, SelectColor);
                /*foreach (Entity e in node.Occupants)
                {
                    DisplayMovement(e);
                }
                */
                result = SelectedNode;
            }
        }
        return result;
    }

    

    public void DeselectNode()
    {
        if (SelectedNode != null)
        {
            SelectedSquare.Die();
            //SelectedNode.GridSquares[MouseInputLayer].GetComponent<SpriteRenderer>().color = DefaultColor;
            //ClearNodes((GridSquareLayers)MouseInputLayer);
            ClearNodes((GridSquareLayers)SelectLayer);
            ClearNodes((GridSquareLayers)PlacementLayer);
            HighlightNode(null);
        }
        SelectedNode = null;
    }

    /*public void UpdateDisplayOnLayer(GridSquareLayers layer)
    {
        GridNode node = null;
        if (layer==GridSquareLayers.Highlight)
        {
            node = HighlightedNode;
        }
        else if (layer==GridSquareLayers.Select)
        {
            node = SelectedNode;
        }
        else if (layer==GridSquareLayers.Placement)
        {
            Entity displayed = sPanel.Displayed;
            DisplayPlacement(displayed);
            return;
        }
        ClearNodes(layer);
        if (node==null)
        {
            return;
        }
        foreach (Entity occupant in node.Occupants)
        {
            if (!occupant.GetAction() && !Input.GetKey(KeyCode.LeftControl))
            {
                continue;
            }
            if (!occupant.isOwner() || Input.GetKey(KeyCode.LeftControl) || occupant.Movement==0)
            {
                DisplayAttack(occupant);
            }
            else
            {
                DisplayMovement(occupant);
            }
        }
    }*/

    private void Update()
    {
        /*GhostEntity ghost = null;
        if (Global.GameInstance!=null)
        {
            ghost=Global.GameInstance.GetSelectedGhost();
        }
        if (ghost != null)
        {
            UpdateDisplayOnLayer(GridSquareLayers.Placement);
        }
        else
        {
            ClearNodes(GridSquareLayers.Placement);
            UpdateDisplayOnLayer(GridSquareLayers.Highlight);
            UpdateDisplayOnLayer(GridSquareLayers.Select);
        }*/
    }
}
