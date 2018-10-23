using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandButton : BasicUI
{
    Image SelectImage;
    Text text;

    [HideInInspector]
    public RectTransform rTransform;

    public void Select(Color SelectedColor)
    {
        SelectImage.color = SelectedColor;
    }
    public void Deselect()
    {
        SelectImage.color = Color.white;
    }
    public void Awake()
    {
        SelectImage=GetComponent<Image>();
        text=GetComponentInChildren<Text>();
        rTransform=GetComponent<RectTransform>();
    }
    public void UpdateButton(bool active)
    {
        gameObject.SetActive(active);
        SelectImage.color = Color.white;
    }
}
