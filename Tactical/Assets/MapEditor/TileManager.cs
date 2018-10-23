using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileEnum { empty, ground, tree, mountain, water, mine, player1, player2}
public enum TileOrigin { centered, bottom }

public class TileManager:MonoBehaviour
{
    const int TileEnumCount = 8;
    public Sprite sprite_ground;
    public Sprite sprite_tree;
    public Sprite sprite_mountain;
    public Sprite sprite_water;
    public Sprite sprite_mine;
    public Sprite player1;
    public Sprite player2;

    public delegate Sprite GetTerrainObjectDelegate(GridNode node);
    public static GetTerrainObjectDelegate[] TileDictionary = new GetTerrainObjectDelegate[TileEnumCount];
    public static TileOrigin [] TileOriginDictionary = new TileOrigin[TileEnumCount];
    private void Awake()
    {
        TileOriginDictionary[(int)TileEnum.empty] = TileOrigin.centered;
        TileOriginDictionary[(int)TileEnum.ground] = TileOrigin.centered;
        TileOriginDictionary[(int)TileEnum.water] = TileOrigin.centered;
        TileOriginDictionary[(int)TileEnum.tree] = TileOrigin.bottom;
        TileOriginDictionary[(int)TileEnum.mountain] = TileOrigin.bottom;
        TileOriginDictionary[(int)TileEnum.mine] = TileOrigin.bottom;
        TileOriginDictionary[(int)TileEnum.player1] = TileOrigin.centered;
        TileOriginDictionary[(int)TileEnum.player2] = TileOrigin.centered;

        TileDictionary[(int)TileEnum.empty] = GetEmptySprite;
        TileDictionary[(int)TileEnum.ground] = GetGroundSprite;
        TileDictionary[(int)TileEnum.tree] = GetTreeSprite;
        TileDictionary[(int)TileEnum.mountain] = GetMountainSprite;
        TileDictionary[(int)TileEnum.water] = GetWaterSprite;
        TileDictionary[(int)TileEnum.mine] = GetMineSprite;
        TileDictionary[(int)TileEnum.player1] = GetP1Sprite;
        TileDictionary[(int)TileEnum.player2] = GetP2Sprite;
    }

    Sprite GetEmptySprite(GridNode node)
    {
        return sprite_ground;
    }
    Sprite GetGroundSprite(GridNode node)
    {
        return sprite_ground;
    }

    Sprite GetTreeSprite(GridNode node)
    {
        return sprite_tree;
    }
    Sprite GetMountainSprite(GridNode node)
    {
        return sprite_mountain;
    }
    Sprite GetWaterSprite(GridNode node)
    {
        return sprite_water;
    }
    Sprite GetMineSprite(GridNode node)
    {
        return sprite_mine;
    }

    Sprite GetP1Sprite(GridNode node)
    {
        return player1;
    }
    Sprite GetP2Sprite(GridNode node)
    {
        return player2;
    }
}
