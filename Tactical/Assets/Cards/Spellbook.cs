using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour {
    public GameObject SpellbookContent;
    public GameObject CardPrefab;
    public List<UICard> CardList = new List<UICard>();
    public List<Entity> DefaultDeck = new List<Entity>();
    public List<Entity> Deck = new List<Entity>();

    private void Awake()
    {
        Global.spellbook = this;
        gameObject.SetActive(false);
    }
    // Use this for initialization
    void Start () {
        Deck = DefaultDeck;
        if (Global.Card_Manager!=null)
        {
            Deck = Global.Card_Manager.Deck;
        }
        foreach (Entity card in Deck)
        {
            GameObject card_object = Instantiate(CardPrefab, SpellbookContent.transform);
            UICard card_ui = card_object.GetComponent<UICard>();
            card_ui.SetCard(card);
            CardList.Add(card_ui);
        }
	}

    public bool UseCard(Entity card)
    {
        for (int i=0; i<CardList.Count; i++)
        {
            if (CardList[i].GetCard()==card)
            {
                Destroy(CardList[i].gameObject);
                CardList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
