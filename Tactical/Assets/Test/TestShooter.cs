using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShooter : MonoBehaviour {
    public GameObject projectilePrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            GameObject go=Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            TestProjectile tp=go.GetComponent<TestProjectile>();
            tp.SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
	}
}
