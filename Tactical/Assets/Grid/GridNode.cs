using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode {
    //public Dictionary<int, GridSquare> GridSquares = new Dictionary<int, GridSquare>();
    public Grid grid;
    public const int GroundLayer = 0;
    public const int HighlightLayer = 1;
    public const int SelectLayer = 2;
    public const int PlacementLayer = 3;
    public const int MouseInput = 4;
    public const int BaseLayerSize = 5;

    public const int TerrainLayer= 0;
    public const int BuildingLayer = 1;
    public const int EntityLayer = 2;
    public const int ProjectileLayer = 3;

    public TileEnum tile_enum = TileEnum.empty;

    GridSquare BaseSquare;
    TerrainEntity terrain_entity;

    public Vector3 position;
    public GameObject GridSquareEntity;
    public List<Entity> Occupants = new List<Entity>();
    public Vector2Int GridIndex;

    public GridNode(Vector3 position, int x, int y)
    {
        this.position = position;
        GridIndex = new Vector2Int(x, y);
    }

    public int GetOrder()
    {
        return BaseLayerSize+(grid.GridSize.y - GridIndex.y)*100;
    }

    public void SetTile(TileEnum te)
    {
        tile_enum = te;
        SetTile();
    }

    public void SetTile()
    {
        SetBaseSquare();
        SetTerrainEntity();
    }

    public void SetBaseSquare()
    {
        if (tile_enum==TileEnum.empty)
        {
            if (BaseSquare != null)
            {
                BaseSquare.Die();
                BaseSquare = null;
            }
            return;
        }
        if (BaseSquare == null)
        {
            GridSquare gs = grid.CreateGridSquare(this, GetOrder(), grid.GridSquareColor);
            BaseSquare = gs;
        }
        BaseSquare.sprite_renderer.sprite = TileManager.TileDictionary[(int)TileEnum.ground].Invoke(this);
        BaseSquare.sprite_renderer.sortingOrder = GroundLayer;
    }

    public void SetTerrainEntity()
    {
        if (tile_enum == TileEnum.ground || tile_enum==TileEnum.empty) 
        {
            if (terrain_entity != null)
            {
                terrain_entity.Die();
            }
            return;
        }
        if (terrain_entity == null)
        {
            GameObject go = GameObject.Instantiate(grid.TerrainEntityPrefab, grid.transform);
            terrain_entity = go.GetComponent<TerrainEntity>();
        }
        terrain_entity.SetTile(this);
        terrain_entity.sprite_renderer.sortingOrder = GetOrder() + TerrainLayer;
    }

    public TerrainEntity GetTerrainEntity()
    {
        return terrain_entity;
    }

    /*public bool AddSquare(int order, GameObject square)
    {
        square.transform.position = this.position;
        square.GetComponent<SpriteRenderer>().sortingOrder = order;
        if (!GridSquares.ContainsKey(order))
        {
            GridSquare gs=square.GetComponent<GridSquare>();
            GridSquares.Add(order, gs);
            return true;
        }
        return false;
    }*/

    public void Die()
    {
        /*foreach(KeyValuePair<int, GridSquare> kvp in GridSquares)
        {
            kvp.Value.Die();
        }*/
        if (terrain_entity != null)
        {
            terrain_entity.Die();
        }
        if (BaseSquare!=null)
        {
            BaseSquare.Die();
        }
    }
}
