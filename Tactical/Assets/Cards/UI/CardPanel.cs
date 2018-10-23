using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour {
    CardPanelCard HighlightedCard;
    public Color DefaultColor = Color.white;
    public Color HighlightColor = Color.yellow;

    public void HighlightCard(CardPanelCard card)
    {
        if (HighlightedCard != null)
        {
            HighlightedCard.GetComponent<Image>().color = DefaultColor;
        }
        if (card != null)
        {
            card.GetComponent<Image>().color = HighlightColor;
        }
        HighlightedCard = card;

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
