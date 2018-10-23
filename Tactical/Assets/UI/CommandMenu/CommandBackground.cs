using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBackground : MonoBehaviour {

    public float Padding;
    public CommandMenu menu;
    public RectTransform rTransform;

    private void Update()
    {
        float height = menu.GetHeight();
        if (height==0)
        {
            rTransform.sizeDelta = new Vector3(rTransform.sizeDelta.x, 0, 1);
        }
        else
        {
            rTransform.sizeDelta = new Vector3(rTransform.sizeDelta.x, height + Padding * 2, 1);
        }
        
    }
}
