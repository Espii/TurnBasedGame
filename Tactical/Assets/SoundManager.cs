using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    static bool created;
    public AudioSource MenuClick;
	// Use this for initialization
	void Awake () {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            Global.sound_manager = this;
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
	public void PlayAudio(AudioSource audio)
    {
        if (audio==null)
        {
            return;
        }
        if (audio.clip != null)
        {
            audio.Play();
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
