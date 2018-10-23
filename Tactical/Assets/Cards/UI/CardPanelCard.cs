using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardPanelCard : MonoBehaviour, IPointerClickHandler {
    public CardPanel panel;
    public Entity card;
    public Image img;
	// Use this for initialization

    public void OnPointerClick(PointerEventData data)
    {
        panel.HighlightCard(this);
    }
	void Start () {
        img.sprite = card.CardImage;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
