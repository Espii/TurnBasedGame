using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorManager : MonoBehaviour {
    public Grid gamegrid;

    public string SerializeMap()
    {
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < gamegrid.GridSize.y; y++)
        {
            for (int x = 0; x < gamegrid.GridSize.x; x++)
            {
                GridNode node = gamegrid.GetNode(x, y);
                sb.Append((int)node.tile_enum);
                if (gamegrid.GetNode(x + 1, 0) != null)
                {
                    sb.Append(",");
                }
            }
            if (gamegrid.GetNode(0, y+1) != null)
            {
                sb.AppendLine();
            }
        }
        return sb.ToString();
    }

    public void SaveMap()
    {
        string serializedMap = SerializeMap();
        System.IO.File.WriteAllText("Assets/Resources/Map.txt", serializedMap);
    }

    void ReadString()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
    public void LoadMap()
    {
        gamegrid.LoadMap();
    }
	// Use this for ini66tialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
