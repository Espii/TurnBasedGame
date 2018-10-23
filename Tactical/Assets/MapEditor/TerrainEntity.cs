using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TerrainEntity : MonoBehaviour {
    public SpriteRenderer sprite_renderer;
    public TileEnum tile_enum;
    public GridNode node;


    public void Hide()
    {
        sprite_renderer.enabled = false;
    }

    public void Show()
    {
        sprite_renderer.enabled = true;
    }
    public void SetTile(GridNode node)
    {
        if (node.tile_enum==TileEnum.ground)
        {
            Die();
            return;
        }
        this.node = node;
        this.tile_enum = node.tile_enum;
        transform.position = node.position;
        sprite_renderer.sprite = TileManager.TileDictionary[(int)node.tile_enum].Invoke(node);
        if (node.tile_enum == TileEnum.player1)
        {
            sprite_renderer.color = Color.red;
        }
        else if (node.tile_enum == TileEnum.player2)
        {
            sprite_renderer.color = Color.blue;
        }
        if (TileManager.TileOriginDictionary[(int)tile_enum] == TileOrigin.bottom)
        {
            sprite_renderer.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        else
        {
            sprite_renderer.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
