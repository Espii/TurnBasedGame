using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanningCamera : MonoBehaviour {

    public float PanSpeed=10;
    public Grid grid;
	// Use this for initialization
	void Start () {
        float x = grid.GridWorldSize.x + grid.transform.position.x;
        float y= grid.GridWorldSize.y + grid.transform.position.y;
        float z = transform.position.z;
        transform.position = new Vector3(x/2,y/2,z);
	}
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        transform.position += new Vector3(x, y, 0).normalized * PanSpeed * Time.deltaTime;
        //SetCamera(Left, Right, Up, Down);
    }
}
