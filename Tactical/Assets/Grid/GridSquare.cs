using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquare : MonoBehaviour {
    public GridNode node;
    public SpriteRenderer sprite_renderer;


    public void Die()
    {
        Destroy(gameObject);
    }
}
