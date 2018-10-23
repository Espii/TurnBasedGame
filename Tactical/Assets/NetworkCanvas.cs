using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCanvas : MonoBehaviour {
    static bool created;
    public GameObject MessageBoxPrefab;
    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            Global.NetCanvas = this;
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public MessageBox CreateMessageBox(string title, string message, string button, MessageBox.ButtonCallback button_callback)
    {
        GameObject go = Instantiate(MessageBoxPrefab, transform);
        MessageBox msgBox = go.GetComponent<MessageBox>();
        msgBox.SetMessageBox(title, message, button, button_callback);
        return msgBox;
    }
}
