using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LoginInput : MonoBehaviour {
    public InputField UserInput;
    public InputField PassInput;
    public GameObject LoginPanel;
    public GameObject ConnectPanel;

    public void Connect()
    {
        SoundManager sManager = Global.sound_manager;
        sManager.PlayAudio(sManager.MenuClick);
        Global.NetManager.Connect();
    }
	// Use this for initialization1
	void Awake () {
        Global.login_panel = this;
    }
}
