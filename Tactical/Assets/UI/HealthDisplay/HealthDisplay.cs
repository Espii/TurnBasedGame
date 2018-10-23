using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    public float MoveSpeed = 5;
    public int DisplayedAmount = 0;
    public float Duration = 2;
    public Text text;
    public Entity target;
    public float OffsetY=0;
    public float TimeAlive = 0;
    public Vector3 target_position;
    public Color Positive=Color.green;
    public Color Negative=Color.red;
    public void SetTarget(Entity target, int amt)
    {
        this.target = target;
        DisplayedAmount = amt;
        text.text = amt.ToString();
        if (amt > 0)
        {
            text.color = Positive;
        }
        else
        {
            text.color = Negative;
        }        
        target_position = target.transform.position;
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
        
        if (target!=null)
        {
            target_position = target.transform.position;
        }
        OffsetY += Time.deltaTime * MoveSpeed;
        TimeAlive += Time.deltaTime;
        if (TimeAlive >= Duration)
        {
            Die();
        }
        Vector3 position = new Vector3(target_position.x, target_position.y+OffsetY, target_position.z);
        Vector3 screen_position=Global.MainCamera.WorldToScreenPoint(position);
        transform.position = screen_position;
	}
}
