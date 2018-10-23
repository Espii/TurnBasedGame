using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Entity Owner;
    public Entity Target;
    public string ImpactAnimation;
    public float Speed = 1;
    public float CloseEnough = 0.05f;
    public Animator anim;
    public GridNode node;
    public SpriteRenderer mySpriteRenderer;
    bool Traversing = true;

    public void Die()
    {
        Destroy(gameObject);
    }

    float GetRotation(Vector3 Direction)
    {
        float Deg = Vector3.Angle(Vector3.right, Direction);
        if (Direction.y < 0)
        {
            return -Deg;
        }
        return Deg;
    }

    void UpdateLayer()
    {
        node = Global.GameGrid.GetNodeFromPosition(transform.position);
        if (node==null)
        {
            return;
        }
        mySpriteRenderer.sortingOrder = node.GetOrder() + GridNode.ProjectileLayer;
    }

    void UpdateRotation()
    {
        Vector3 Direction = Target.transform.position - transform.position;
        Direction.Normalize();
        float Rotation = GetRotation(Direction);
        transform.eulerAngles = new Vector3(0, 0, Rotation);
    }

    // Use this for initialization
    void Start () {
        if (Owner == null || Target == null)
        {
            Die();
        }
        UpdateLayer();
        UpdateRotation();
    }
	
	// Update is called once per frame
	void Update () {
        if (Traversing)
        {
            Vector3 direction = (Target.transform.position - transform.position).normalized;
            if ((Target.transform.position - transform.position).sqrMagnitude > CloseEnough * CloseEnough)
            {
                transform.position += direction * Speed * Time.deltaTime;
                UpdateLayer();
                UpdateRotation();
            }
            else
            {
                transform.position = Target.transform.position;
                UpdateLayer();
                List<DistanceFromNodeResult> NodeResults=Global.GameGrid.GetNodesBFSDistanceFromNode(Target.GetNode(), Owner.AreaAttackRange);
                foreach (DistanceFromNodeResult result in NodeResults)
                {
                    foreach(Entity entity in result.node.Occupants)
                    {
                        entity.ChangeHealthByAmount(-Owner.Attack);
                    }
                    
                }
                if (!AnimatorManager.PlayAnimation(anim, ImpactAnimation))
                {
                    Die();
                }
                Traversing = false;
            }
        }
        else
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (!info.IsName(ImpactAnimation))
            {
                Die();
            }
        }
	}
}
