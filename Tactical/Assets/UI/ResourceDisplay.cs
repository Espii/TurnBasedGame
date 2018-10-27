using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceDisplay : MonoBehaviour {
    public PlayerResourceManager resource_manager;
    public TextMeshProUGUI ResourceText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ResourceText.text = "Gold: " + resource_manager.GetResourceString();
	}
}
