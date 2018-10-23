using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageBox : MonoBehaviour {
    public Text title;
    public Text message;
    public Text button;
    ButtonCallback button_callback;

    public delegate void ButtonCallback();


    public void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    public void SetMessageBox(string title, string message, string button, ButtonCallback button_callback)
    {
        this.title.text = title;
        this.message.text = message;
        this.button.text = button;
        this.button_callback = button_callback;
    }

    public void Die()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
        if (this != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnSceneChange(Scene current, Scene next)
    {
        Die();
    }
    public void ButtonClick()
    {
        if (this.button_callback!=null)
        {
            button_callback.Invoke();
        }
        Die();
    }
}
