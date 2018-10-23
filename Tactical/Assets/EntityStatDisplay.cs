using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityStatDisplay : MonoBehaviour {
    Entity target = null;
    public Text HealthText;
    public Text AttackText;
    public Image HealthBackground;
    public Image AttackBackground;
    public RectTransform myRectTransform;
    public Color FriendlyColor = Color.blue;
    public Color HostileColor = Color.red;
    public void SetTarget(Entity target)
    {
        this.target = target;
        Color BackgroundColor = Color.white;
        if (target.PlayerID == Global.PlayerID)
        {
            BackgroundColor = FriendlyColor;
        }
        else
        {
            BackgroundColor = HostileColor;
        }
        HealthBackground.color = BackgroundColor;
        AttackBackground.color = BackgroundColor;
    }

    public void UpdateStatDisplay()
    {
        if (target == null)
        {
            return;
        }
        myRectTransform.sizeDelta=GetSize();
        myRectTransform.position= Global.MainCamera.WorldToScreenPoint(target.transform.position);
        HealthText.text = target.GetHealth().ToString();
        AttackText.text = target.Attack.ToString();
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
