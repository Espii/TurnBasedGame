using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {
    //public Dictionary<int, Entity> CardDB = new Dictionary<int, Entity>();
    public Entity[] CardDB;
    public List<Entity> Deck = new List<Entity>();
    static bool created = false;
    // Use this for initialization
    private void Awake()
    {
        if (created)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        created = true;
        Global.Card_Manager = this;

        /*Object [] CardList=Resources.LoadAll("Cards", typeof(Entity));
        foreach (Object obj in CardList)
        {
            Entity card = obj as Entity;
            if (!CardDB.ContainsKey(card.CardID))
            {
                CardDB.Add(card.CardID, card);
            }
        }

        foreach(KeyValuePair<int, Entity> entry in CardDB)
        {
            CardDB_Inspector.Add(entry.Value);
        }*/

        for (int i = 0; i < CardDB.Length; i++)
        {
            CardDB[i].CardID = i;
        }
        Debug.Log(CardDB);
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
