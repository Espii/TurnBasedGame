using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public float PanSpeed = 10;
    public float Padding = 0;
    public float MinZoom = 0.1f;
    public float MaxZoom = 5;
    public float ZoomRate = 50;
    public Camera MainCamera;
    public Grid grid;
    public bool MouseDown = false;
    public Vector3 MouseDownPosition;

    void MoveCamera()
    {
        if (MouseDown && MouseDownPosition!=Input.mousePosition)
        {
            Vector3 offset = Global.MainCamera.ScreenToWorldPoint(MouseDownPosition)-Global.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            Global.MainCamera.transform.position += offset;
            MouseDownPosition = Input.mousePosition;
        }
        else
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            transform.position += new Vector3(x, y, 0).normalized * PanSpeed * Time.deltaTime;
            UpdateCameraPositionWithinBoundary();
        }
    }

    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
        UpdateCameraPositionWithinBoundary();
    }

    public void SetPosition2D(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        UpdateCameraPositionWithinBoundary();
    }

    void UpdateCameraPositionWithinBoundary()
    {
        return; //ignore keep in bounds
        float HalfWidth = GetHalfWidth();
        float HalfHeight = MainCamera.orthographicSize;
        float CameraLeft = transform.position.x - HalfWidth;
        float CameraRight = transform.position.x + HalfWidth;
        float CameraUp = transform.position.y + HalfHeight;
        float CameraDown = transform.position.y - HalfHeight;
        float GridLeft = grid.transform.position.x - Padding;
        float GridRight = grid.transform.position.x + grid.GridWorldSize.x + Padding;
        float GridUp = grid.transform.position.y + grid.GridWorldSize.y + Padding;
        float GridDown = grid.transform.position.y - Padding;

        if (CameraLeft<GridLeft)
        {
            transform.position = new Vector3(GridLeft+HalfWidth, transform.position.y, transform.position.z);
        }
        else if (CameraRight > GridRight)
        {
            transform.position = new Vector3(GridRight-HalfWidth, transform.position.y, transform.position.z);
        }
        if (CameraDown < GridDown)
        {
            transform.position = new Vector3(transform.position.x, GridDown + HalfHeight, transform.position.z);
        }
        else if (CameraUp > GridUp)
        {
            transform.position = new Vector3(transform.position.x, GridUp - HalfHeight, transform.position.z);
        }
    }

    float GetHalfWidth()
    {
        return MainCamera.orthographicSize * Screen.width / Screen.height;
    }
	// Use this for initialization
	void Start () {
        Global.myCameraManager = this;
        /*
        float x = grid.GridWorldSize.x + grid.transform.position.x;
        float y= grid.GridWorldSize.y + grid.transform.position.y;
        float z = transform.position.z;
        transform.position = new Vector3(x/2,y/2,z);
        */
	}

    void ZoomCamera()
    {
        if (Global.build_menu.GetDisplayed())
        {
            return;
        }
        Global.MainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ZoomRate * Global.MainCamera.orthographicSize * Time.deltaTime;
        if (Global.MainCamera.orthographicSize < MinZoom)
        {
            Global.MainCamera.orthographicSize = MinZoom;
        }
        if (Global.MainCamera.orthographicSize > MaxZoom)
        {
            Global.MainCamera.orthographicSize = MaxZoom;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(2))
        {
            MouseDown = true;
            MouseDownPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            MouseDown = false;
        }
        ZoomCamera();
        MoveCamera();
    }
}
