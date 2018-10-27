using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityStatDisplay : MonoBehaviour {
    Entity target = null;
    public Image AttackBackground;
    public Image HealthBackground;
   
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI HealthText;
    public RectTransform myRectTransform;
    public Color FriendlyColor = Color.blue;
    public Color HostileColor = Color.red;

    public void SetTarget(Entity target)
    {
        this.target = target;
        Color Color = Color.white;
        if (target.PlayerID == Global.PlayerID)
        {
            Color = FriendlyColor;
        }
        else
        {
            Color = HostileColor;
        }

        AttackBackground.color = Color;
        HealthBackground.color = Color;
        //AttackText.faceColor = Color;
        //HealthText.faceColor = Color;
    }




    public void UpdateStatDisplay()
    {
        if (target == null)
        {
            return;
        }

        Vector2 NewSize = GetSize();
        Vector2 ActualSize = myRectTransform.sizeDelta * myRectTransform.localScale;
        if (NewSize != ActualSize)
        {
            ScaleDisplay(NewSize);
        }
        myRectTransform.position= Global.MainCamera.WorldToScreenPoint(target.transform.position);
        HealthText.text = target.GetHealth().ToString();
        AttackText.text = target.Attack.ToString();
    }

    Vector3 V3Divide(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }
    void ScaleDisplay(Vector2 NewSize)
    {
        transform.localScale = NewSize / myRectTransform.sizeDelta;
        /*
        AttackText.transform.localScale *= ratio;
        HealthText.transform.localScale *= ratio;
        //ScaleText(AttackText, ratio);
        //ScaleText(HealthText, ratio);
        myRectTransform.sizeDelta = NewSize;
        */
    }


    void ScaleText(TextMeshProUGUI text, float ratio)
    {
        text.margin *= ratio;
        text.fontSize *= ratio;
    }

    public Vector2 GetSize()
    {
        Vector3 zero = new Vector3(0, 0);
        Vector3 right = new Vector3(Global.GameGrid.GridSquareSize.x, Global.GameGrid.GridSquareSize.y);
        Vector3 scr_zero = Global.MainCamera.WorldToScreenPoint(zero);
        Vector3 scr_right = Global.MainCamera.WorldToScreenPoint(right);
        Vector2 size = (scr_right-scr_zero);
        return Abs(size);
    }

    public Vector2 Abs(Vector2 v) {
        return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateStatDisplay();
	}
}
