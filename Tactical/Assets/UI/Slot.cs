using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    SlotManager manager;
    public Image slot_image;
    public GameObject HighlightImage;
    public object content;
    public RectTransform SlotImageContainer;

    public void SetContent(object content, Sprite sprite)
    {
        this.content = content;
        if (sprite==null)
        {
            slot_image.sprite = null;
            return;
        }
        slot_image.rectTransform.sizeDelta=Global.GetImageSize(SlotImageContainer, sprite);
        slot_image.sprite = sprite;
    }

    public void SetHighlight(bool highlight)
    {
        HighlightImage.SetActive(highlight);
    }

    public void OnPointerClick(PointerEventData pData)
    {
        manager.SelectSlot(this);
    }

    public void OnPointerEnter(PointerEventData pData)
    {
        manager.MouseOverEnter(this);
    }

    public void OnPointerExit(PointerEventData pData)
    {
        manager.MouseOverExit(this);
    }

    public void SetManager(SlotManager manager)
    {
        this.manager = manager;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
