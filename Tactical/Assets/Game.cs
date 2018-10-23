using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    Entity SelectedCard;
    GhostEntity SelectedGhost = null;
    public Button EndTurnButton;
    public GameTimer game_timer;
    public PlayerResourceManager myPlayerResourceManager = null;
    //public GameObject EntityPrefab;
    public GameObject GhostPrefab;
    //public Text TurnText;
    public bool CurrentTurn = false;
    public GridVisual gVisual = null;
    public Dictionary<int, Entity> EntityList = new Dictionary<int, Entity>();
    public GameObject HealthDisplayPrefab;
    public Canvas cCanvas;
    public ScalingCanvas sCanvas;

    public void PlayCard(Entity card, int x, int y)
    {
        card.PlayerID = Global.PlayerID;
        GridNode node = Global.GameGrid.GetNode(x, y);
        if (!card.isWalkable(node))
        {
            return;
        }
        if (!myPlayerResourceManager.PlayCard(card))
        {
            return;
        }
        /*if (Global.spellbook.UseCard(card))
        {
            Global.NetManager.Send(RequestEntityPacket.GetBytes(card.CardID, x, y));
        }*/
        Global.NetManager.Send(RequestEntityPacket.GetBytes(card.CardID, x, y));
        SelectCard(null);
    }

    public void PlaySelectedCard(int x, int y)
    {
        if (!CurrentTurn || SelectedCard == null)
        {
            return;
        }
        PlayCard(SelectedCard, x, y);
    }
    public void SelectCard(Entity card)
    {
        SelectedCard = card;
        if (SelectedGhost != null)
        {
            SelectedGhost.Die();
        }
        if (card!=null)
        {
            SelectedGhost=Instantiate(GhostPrefab, transform).GetComponent<GhostEntity>();
            SelectedGhost.SetCard(SelectedCard);
        }
        gVisual.DisplayPlacement(gVisual.GetSelectedEntity());
    }

    public Entity GetSelectedCard()
    {
        return SelectedCard;
    }

    public GhostEntity GetSelectedGhost()
    {
        return SelectedGhost;
    }
    public void EndTurn()
    {
        //TurnText.text = "";
        if (CurrentTurn)
        {
            Global.NetManager.Send_EndTurn();
            game_timer.StartTimer();
        }
        CurrentTurn = false;
        EndTurnButton.gameObject.SetActive(false);
    }
    public void StartTurn()
    {
        myPlayerResourceManager.StartTurn();
        Global.MouseManager.DeselectNode();
        game_timer.StartTimer();
        List<int> RemoveKeyList = new List<int>();
        foreach (KeyValuePair<int, Entity> kvp in EntityList)
        {
            if (kvp.Value == null)
            {
                RemoveKeyList.Add(kvp.Key);
                continue;
            }
            kvp.Value.StartTurn();
        }
        foreach (int key in RemoveKeyList)
        {
            EntityList.Remove(key);
        }
        //TurnText.text = "Current Turn";
        Global.GameInstance.CurrentTurn = true;
        EndTurnButton.gameObject.SetActive(true);
    }

    private void Awake()
    {
        Global.GameInstance = this;
    }

    public void CreateEntity(int PlayerID, int NetID, Entity card, Vector2Int position)
    {
        GameObject unit = Instantiate(card.gameObject, Global.GameGrid.grid[position.x, position.y].position, Quaternion.identity);
        Entity e=unit.GetComponent<Entity>();
        e.SetNode(Global.GameGrid.grid[position.x, position.y]);
        e.InitializeEntity(PlayerID, NetID);
        //e.NetID = NetID;
        //e.PlayerID = PlayerID;
        //e.InitializeEntity(PlayerID, NetID, card);
    }

    // Use this for initialization
    void Start () {
        //GameObject unit = Instantiate(UnitPrefab, Global.GameGrid.grid[0, 0].position, Quaternion.identity);
        //unit.GetComponent<Entity>().SetNode(Global.GameGrid.grid[0, 0]);
    }
	
	// Update is called once per frame
	void Update () {
	}
}
