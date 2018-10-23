using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GridSquareLayers { Base, Highlight, Select, Placement }

public class DistanceFromNodeResult
{
    public int distance;
    public GridNode node;
    public GridNode parent;

    public DistanceFromNodeResult(int distance, GridNode node, GridNode parent=null)
    {
        this.distance = distance;
        this.node = node;
        this.parent = parent;

    }
    public static int Contains(List<DistanceFromNodeResult> l, GridNode n) //returns index if found, returns -1 if not
    {
        for (int i=0; i<l.Count; i++)
        {
            DistanceFromNodeResult res = l[i];
            if (res.node==n)
            {
                return i;
            }
        }
        return -1;
    }
}

public class Grid : MonoBehaviour {
    public TileManager tile_manager;
    public GridVisual gVisual;
    public Vector2Int GridSize= new Vector2Int(0, 0);
    public Vector2 GridSquareSize = new Vector2(1, 1);
    public Color GridSquareColor = Color.white;
    public GameObject PlayerHQPrefab;
    public GameObject MapSizeIndicator;
    Entity PlayerHQ;

    [SerializeField]
    GameObject GridSquarePrefab;

    public GameObject TerrainEntityPrefab;
    public CameraManager camera_manager;
    //public Vector2Int StartPosition1 = new Vector2Int(0, 0);
    //public Vector2Int StartPosition2 = new Vector2Int(0, 0);

    public Vector2 GridWorldSize;
    public GridNode[,] grid;


    public GridSquare CreateGridSquare(GridNode node, int order, Color color)
    {
        GameObject go = Instantiate(GridSquarePrefab, transform);
        GridSquare gs = go.GetComponent<GridSquare>();
        gs.node = node;
        gs.transform.position = node.position;
        gs.sprite_renderer.sortingOrder = order;
        gs.sprite_renderer.color = color;
        return gs;
    }
    GridNode [,] CreateGrid(int GridWidth, int GridHeight, float SquareWidth, float SquareHeight)
    {
        GridNode [,] grid = new GridNode[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                float xPos = x * SquareWidth + SquareWidth / 2;
                float yPos = y * SquareHeight + SquareHeight / 2;
                float zPos = this.transform.position.z;
                GridNode node = new GridNode(new Vector3(xPos, yPos, zPos), x, y);
                node.grid = this;
                grid[x, y] = node;



                //node.SetTile();

                /*GameObject highlight = Instantiate(GridSquarePrefab, this.transform);
                highlight.GetComponent<SpriteRenderer>().color = gVisual.DefaultColor;

                GameObject select = Instantiate(GridSquarePrefab, this.transform);
                select.GetComponent<SpriteRenderer>().color = gVisual.DefaultColor;

                GameObject placement = Instantiate(GridSquarePrefab, this.transform);
                placement.GetComponent<SpriteRenderer>().color = gVisual.DefaultColor;

                GameObject mouse = Instantiate(GridSquarePrefab, this.transform);
                mouse.GetComponent<SpriteRenderer>().color = gVisual.DefaultColor;

                node.AddSquare((int)GridSquareLayers.Base, square);
                node.AddSquare((int)GridSquareLayers.Highlight, highlight);
                node.AddSquare((int)GridSquareLayers.Select, select);
                node.AddSquare((int)GridSquareLayers.Placement, placement);
                node.AddSquare((int)GridSquareLayers.MouseInput, mouse);*/
            }
        }
        return grid;
    }

    Vector2 SetGridWorldSize(int GridWidth, int GridHeight, float SquareWidth, float SquareHeight)
    {
        float SizeX = GridWidth * SquareWidth;
        float SizeY = GridHeight * SquareHeight;
        return new Vector2(SizeX, SizeY);
    }

    public void SetTerrainEntity(GridNode node, TileEnum tile_enum)
    {
        node.tile_enum = tile_enum;
        node.SetTerrainEntity();
    }

    bool Between01(float num)
    {
        if (num<0 || num>1)
        {
            return false;
        }
        return true;
    }
    public GridNode GetNode(int x, int y)
    {
        if (x < 0 || x >= grid.GetLength(0))
        {
            return null;
        }
        else if (y<0 || y>=grid.GetLength(1))
        {
            return null;
        }
        return grid[x, y];
    }

    public GridNode GetNodeFromMousePosition(Vector3 mouse_position)
    {
        if (camera_manager != null)
        {
            Vector3 position = this.camera_manager.MainCamera.ScreenToWorldPoint(mouse_position);
            return GetNodeFromPosition(position);
        }
        return null;
    }
    public GridNode GetNodeFromPosition(Vector3 position)
    {
        float PercentX = (position.x - transform.position.x) / GridWorldSize.x;
        float PercentY = (position.y - transform.position.y) / GridWorldSize.y;
        int x = (int) (PercentX * GridSize.x);
        int y = (int) (PercentY * GridSize.y);
        //Debug.Log("x: "+ PercentX + " y:"+ PercentY);
        if (!Between01(PercentX) || !Between01(PercentY))
        {
            return null;
        }
        return GetNode(x, y);
    }

    public static int GetDistance(GridNode n1, GridNode n2)
    {
        int x=n1.GridIndex.x - n2.GridIndex.x;
        int y=n1.GridIndex.y - n2.GridIndex.y;
        return Mathf.Abs(x) + Mathf.Abs(y);
    }

    public List<DistanceFromNodeResult> GetNodesBFSDistanceFromNode(GridNode node, int distance)
    {
        return GetNodesBFSDistanceFromNode(null, node, distance);
    }

    bool WalkableTile(GridNode node)
    {
        TileEnum tile = node.tile_enum;
        if (tile == TileEnum.ground)
        {
            return true;
        }
        if (tile == TileEnum.player1)
        {
            return true;
        }
        if (tile == TileEnum.player2)
        {
            return true;
        }
        return false;
    }

    public List<DistanceFromNodeResult> GetNodesBFSDistanceFromNode(Entity entity, GridNode node, int distance)
    {
        List<DistanceFromNodeResult> OpenSet = new List<DistanceFromNodeResult>();
        List<DistanceFromNodeResult> ClosedSet = new List<DistanceFromNodeResult>();
        if (distance==0)
        {
            ClosedSet.Add(new DistanceFromNodeResult(0, node));
            return ClosedSet;
        }

        OpenSet.Add(new DistanceFromNodeResult(0, node));
        while (OpenSet.Count > 0)
        {
            DistanceFromNodeResult CurrentNode = OpenSet[0];
            OpenSet.RemoveAt(0);
            ClosedSet.Add(CurrentNode);
            foreach (GridNode AdjacentNode in GetAdjacentNodes(CurrentNode.node))
            {
                if (entity!=null)
                {

                    if (!entity.isWalkable(AdjacentNode))
                    {
                        Entity enemy = Global.GetEnemyEntityOnNode(AdjacentNode);
                        if (enemy != null && CurrentNode.distance+1<=distance)
                        {
                            ClosedSet.Add(new DistanceFromNodeResult(CurrentNode.distance + 1, AdjacentNode, CurrentNode.node));
                            continue;
                        }
                        else if (!WalkableTile(AdjacentNode))
                        {
                            continue;
                        }
                                
                    }
                }
                if (DistanceFromNodeResult.Contains(ClosedSet, AdjacentNode) == -1)
                {
                    if (CurrentNode.distance < distance)
                    {
                        if (DistanceFromNodeResult.Contains(OpenSet, AdjacentNode) == -1)
                        {
                            OpenSet.Add(new DistanceFromNodeResult(CurrentNode.distance + 1, AdjacentNode, CurrentNode.node));
                        }
                    }
                    //NextSet.Add(AdjacentNode);
                }
            }
        }
        return ClosedSet;
    }

    List<GridNode> GetAdjacentNodes(GridNode node)
    {
        if (node == null)
        {
            Debug.Log("why null?");
        }
        List<GridNode> result = new List<GridNode>();
        for (int x=-1; x<=1; x++)
        {
            for (int y=-1; y<=1; y++)
            {
                if (x==0 && y==0)
                {
                    continue;
                }
                if (x!=0 && y!=0)
                {
                    continue;
                }
                int abs_x = node.GridIndex.x + x;
                int abs_y = node.GridIndex.y + y;
                if (!CheckValid(abs_x, abs_y))
                {
                    continue;
                }
                result.Add(Global.GameGrid.grid[abs_x, abs_y]);
            }
        }
        return result;
    }

    bool CheckValid(int x, int y)
    {
        if (x<0 || x>=GridSize.x)
        {
            return false;
        }
        if (y < 0 || y>=GridSize.y)
        {
            return false;
        }
        return true;
    }
    public void LoadMap()
    {
        TextAsset map = Resources.Load("Map") as TextAsset;
        if (map==null)
        {
            InitializeGrid();
            return;
        }
        string[] lines = map.text.Split('\n');
        int width = lines[0].Split(',').Length;
        int height = lines.Length;
        GridSize = new Vector2Int(width, height);
        InitializeGrid();
        for (int y = 0; y < lines.Length; y++)
        {
            string[] values = lines[y].Split(',');
            for (int x = 0; x < values.Length; x++)
            {
                GridNode node = GetNode(x, y);
                if (node != null)
                {
                    //Debug.Log(splitmap[x]);
                    node.tile_enum = (TileEnum)int.Parse(values[x]);
                }
            }
        }
        for (int x=0; x<GridSize.x; x++)
        {
            for (int y=0; y<GridSize.y; y++)
            {
                GridNode node = GetNode(x, y);
                if (node!=null)
                {

                    if (node.tile_enum == TileEnum.player1 && Global.PlayerID==0)
                    {                        
                        Global.NetManager.RequestEntity(PlayerHQ.CardID, x, y);
                        camera_manager.SetPosition2D(node.position);
                    }
                    else if (node.tile_enum == TileEnum.player2 && Global.PlayerID == 1)
                    {
                        Global.NetManager.RequestEntity(PlayerHQ.CardID, x, y);
                        camera_manager.SetPosition2D(node.position);
                    }
                    GridNode n = GetNode(x, y);
                    n.SetTile();
                }
            }
        }
    }

    public void InitializeGrid()
    {
        if (grid != null)
        {
            foreach (GridNode node in grid)
            {
                node.Die();
            }
        }
        grid = CreateGrid(GridSize.x, GridSize.y, GridSquareSize.x, GridSquareSize.y);
        GridWorldSize = SetGridWorldSize(GridSize.x, GridSize.y, GridSquareSize.x, GridSquareSize.y);
        if (MapSizeIndicator != null)
        {
            MapSizeIndicator.transform.localScale = new Vector3(GridWorldSize.x, GridWorldSize.y, 1);
            MapSizeIndicator.transform.position = new Vector3(transform.position.x+ GridWorldSize.x/2, transform.position.y + GridWorldSize.y / 2, 0);
        }
    }

    // Use this for initialization
    void Start()
    {
        Global.GameGrid = this;
        PlayerHQ = PlayerHQPrefab.GetComponent<Entity>();
        LoadMap();
    }

    

	// Update is called once per frame
	void Update () {
        
    }
}
