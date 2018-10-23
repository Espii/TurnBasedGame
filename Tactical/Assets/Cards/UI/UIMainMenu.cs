using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : MonoBehaviour {
    public GameObject DeckBuilder;
    private void Awake()
    {
        Global.myUIMainMenu = this;
        this.gameObject.SetActive(false);
    }
    public void OpenDeckBuilder()
    {
        DeckBuilder.SetActive(true);
    }

    public void CloseDeckBuilder()
    {
        DeckBuilder.SetActive(false);
    }

    public void FindMatch()
    {
        SoundManager sManager = Global.sound_manager;
        sManager.PlayAudio(sManager.MenuClick);
        if (!Global.NetManager.FindingMatch)
            Global.NetManager.Send_FindMatch(FindMatchType.game); 
    }
    public void PracticeMatch()
    {
        SoundManager sManager = Global.sound_manager;
        sManager.PlayAudio(sManager.MenuClick);
        if (!Global.NetManager.FindingMatch)
            Global.NetManager.Send_FindMatch(FindMatchType.practice);
    }
}
