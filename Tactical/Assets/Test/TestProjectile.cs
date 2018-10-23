using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour {
    Vector3 Direction = Vector3.zero;

    float GetRotation(Vector3 Direction)
    {
        float Deg = Vector3.Angle(Vector3.right, Direction);
        if (Direction.y<0)
        {
            return -Deg;
        }
        return Deg;
    }

    public float speed = 1;
    public void SetTarget(Vector3 position)
    {
        Direction=new Vector3(position.x, position.y, 0)-transform.position;
        Direction.Normalize();
        float Rotation=GetRotation(Direction);
        transform.eulerAngles = new Vector3(0, 0, Rotation);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position+=Direction*Time.deltaTime* speed;
        float Rotation = GetRotation(Direction);
        transform.eulerAngles = new Vector3(0, 0, Rotation);

    }
}
