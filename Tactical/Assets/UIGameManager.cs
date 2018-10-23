using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameManager: MonoBehaviour {
    public GameObject SpellbookPanel;
    public GameObject GameMenu;

    public void CloseSpellbook()
    {
        SpellbookPanel.SetActive(false);
    }
    public void ToggleSpellbook()
    {
        if (SpellbookPanel.activeSelf)
        {
            SpellbookPanel.SetActive(false);
        }
        else
        {
            SpellbookPanel.SetActive(true);
        }
    }

    public void ToggleGameMenu()
    {
        if (GameMenu.activeSelf)
        {
            GameMenu.SetActive(false);
        }
        else
        {
            GameMenu.SetActive(true);
        }
    }

    public void CloseGameMenu()
    {
       GameMenu.SetActive(false);
    }

    public void handle_game_menu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleGameMenu();
        }
    }
    public void EndGame()
    {
        Global.NetManager.Send(Packet.GetBytes(PacketType.RequestEndGame));
    }
    void Update()
    {
        handle_game_menu();
    }

    void Awake()
    {
        Global.myUIGameManager = this;
    }

    
}
