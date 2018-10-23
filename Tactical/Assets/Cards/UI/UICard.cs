using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICard : MonoBehaviour {
    public RectTransform rTransform;
    Entity card;
    public RectTransform CardImageContainer;
    public Image CardImage;
    public Text txtCardText;
    public Text txtCardStat;
    public Text txtCardCost;
    public Text txtCardName;


    public void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            SetCard(null);
        }
    }

    public void SetPosition(Vector3 ScreenPosition)
    {
        float width = rTransform.sizeDelta.x * this.transform.parent.localScale.x;
        float height = rTransform.sizeDelta.y * this.transform.parent.localScale.y;
        float Left = width * rTransform.pivot.x;
        float Right = width * (1 - rTransform.pivot.x);
        float Down = height * rTransform.pivot.y;
        float Up = height * (1 - rTransform.pivot.y);
        float x = ScreenPosition.x;
        float y = ScreenPosition.y;
        if (ScreenPosition.x - Left < 0)
        {
            x = 0 + Left;
        }
        else if (ScreenPosition.x + Right > Screen.width)
        {
            x = Screen.width - Right;
        }
        if (ScreenPosition.y - Down < 0)
        {
            y = 0 + Down;
        }
        else if (ScreenPosition.y+Up>Screen.height)
        {
            y = Screen.height - Up;
        }
        this.transform.position = new Vector3(x, y, this.transform.position.z);
    }
    public Entity GetCard()
    {
        return card;
    }
    public void SetCard(Entity card)
    {
        if (card == null)
        {
            this.card = null;
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            CardImage.sprite = card.CardImage;
            CardImage.rectTransform.sizeDelta=Global.GetImageSize(CardImageContainer, card.CardImage);
            txtCardStat.text = card.Attack + "/" + card.GetHealth();
            txtCardText.text = card.CardText;
            txtCardName.text = card.name;
            txtCardCost.text = card.cost.ToString();
            this.gameObject.SetActive(true);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
