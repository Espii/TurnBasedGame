using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEntity : MonoBehaviour {
    public Entity card;
    public SpriteRenderer mySpriteRenderer;
    public Animator myAnimator;

    public void Die()
    {
        Destroy(gameObject);
    }
    public void Start()
    {
        if (card!=null)
        {
            SetCard(card);
        }
    }
    public void SetCard(Entity card)
    {
        this.card = card;
        mySpriteRenderer.sprite = card.sprite_renderer.sprite;
        mySpriteRenderer.color=new Color(255,255,255,100);
        myAnimator.runtimeAnimatorController = card.animator.runtimeAnimatorController;
    }

    public void SetPosition(GridNode node)
    {
        if (node==null)
        {
            mySpriteRenderer.enabled = false;
            return;
        }
        mySpriteRenderer.enabled = true;
        transform.position = node.position;
        mySpriteRenderer.sortingOrder = node.GetOrder() + GridNode.EntityLayer;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
