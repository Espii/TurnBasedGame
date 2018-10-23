using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorMouseInput : MonoBehaviour {
    public Grid gamegrid;
    public TilePanel panel;
    public GameObject GridSquarePrefab;  

    void PlaceTile(GridNode node)
    {
        if (node==null)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            TileSlot slot = panel.GetSelectedSlot();
            if (slot != null)
            {
                node.tile_enum = slot.tileEnum;
                node.SetBaseSquare();
                node.SetTerrainEntity();
            }
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        GridNode node = gamegrid.GetNodeFromMousePosition(Input.mousePosition);
        if (node == null)
        {
            return;
        }
        gamegrid.gVisual.HighlightNode(node);
        PlaceTile(node);
        
        
	}
}
