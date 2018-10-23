using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour {
    public enum EntityType
    {
        Building,
        Unit,
        Resource
    }
    public enum AttackType
    {
        Melee,
        Ranged,
        Heal
    }

    private float ActiveAlpha = 1f;
    private float InactiveAlpha = 0.5f;

    public int CardID = -1;
    public int PlayerID = -1;
    public int NetID = -1;

    public bool isMoving = false;
    public bool HQ = false;
    public int cost = 1;
    public int Upkeep = 0;
    public bool block = true;
    public bool SummoningSickness = true;
    public List<Entity> ProductionList = new List<Entity>();
    public int ProductionRange = 0;
    public GameObject projectile;
    public GameObject ProjectileSpawnPoint;

    public AttackType aType = AttackType.Melee;
    public EntityType entity_type = EntityType.Unit;

    private GridNode node;
    public int Attack = 0;
    public int AreaAttackRange = 0;
    public int Heal = 0;

    [SerializeField]
    protected int Health = 1;

    public int MaxHealth = 1;

    public int Movement = 0;
    public int TotalMovement = 0;
    public int Range = 0;
    public float speed = 1;

    public Entity Target;

    public Sprite CardImage;
    public string CardText;

    public SpriteRenderer sprite_renderer;
    public Animator animator;

    public string AttackAnimation;
    public string ProductionAnimation;

    public GameObject StatDisplayPrefab;
    public EntityStatDisplay StatDisplay;
    string CurrentAnimation = null;

    public Direction DefaultDirection = Direction.Right;
    bool action = true;

    GridNode waypoint = null;

    public void SetNetID(int NetID)
    {
        this.NetID = NetID;
    }
    public void StartTurn()
    {
        SetAction(true);
        this.Movement = TotalMovement;
    }

    /*public void ProduceCard(Entity e, int x, int y)
    {
        Global.GameInstance.PlayCard(e, x, y);
    }*/

    public List<DistanceFromNodeResult> GetNodes(int distance)
    {
        return Global.GameGrid.GetNodesBFSDistanceFromNode(this, this.node, distance);
    }

    public void DisplayHealthChange(int amount)
    {
        if (amount==0)
        {
            return;
        }
        Game gi = Global.GameInstance;
        GameObject go=Instantiate(gi.HealthDisplayPrefab, gi.sCanvas.transform);
        HealthDisplay hd=go.GetComponent<HealthDisplay>();
        hd.SetTarget(this, amount);
    }

    public void SetHealth(int health, bool DisplayChange=true)
    {
        if (DisplayChange)
        {
            DisplayHealthChange(health - this.Health);
        }
        this.Health = health;
    }

    public void ChangeHealthByAmount(int amount, bool DisplayChange = true)
    {
        SetHealth(this.Health + amount);
    }

    public int GetHealth()
    {
        return Health;
    }
    public enum Direction
    {
        Right,
        Left
    }

    public bool isWalkable(GridNode node)
    {
        if (entity_type == EntityType.Resource)
        {
            if (node.tile_enum != TileEnum.mine || node.Occupants.Count > 0)
            {
                return false;
            }
            return true;
        }
        if (!block)
        {
            return true;
        }
        if (node.tile_enum!=TileEnum.ground)
        {
            return false;
        }
        foreach (Entity e in node.Occupants)
        {
            if (e.block==true)
            {
                return false;
            }
        }
        return true;
    }
    public void MoveTo(GridNode node)
    {
        if (!isWalkable(node))
        {
            return;
        }
        List<DistanceFromNodeResult> MovementNodes = GetNodes(Movement);
        int index = DistanceFromNodeResult.Contains(MovementNodes, node);
        if (index!=-1)
        {
            Movement -= MovementNodes[index].distance;
            Global.NetManager.Send(MoveEntityPacket.GetBytes(this.NetID, node.GridIndex.x, node.GridIndex.y));
        }
    }

    public Vector3 GetProjectileSpawnPoint()
    {
        if (ProjectileSpawnPoint == null)
        {
            return node.position;
        }
        Vector3 pos = ProjectileSpawnPoint.transform.position;
        if (sprite_renderer.flipX)
        {
            Vector3 pspawn_offset = pos - transform.position;
            Vector3 flipped_pspawn = new Vector3(-pspawn_offset.x, pspawn_offset.y, pspawn_offset.z);
            return transform.position + flipped_pspawn;
        }
        else
        {
            return pos;
        }
    }

    public void SetAction(bool action)
    {
        this.action = action;
        if (action==false)
        {
            Movement = 0;
            if (Global.GameGrid.gVisual.SelectedNode == node)
            {
                Global.command_menu.command.Hide();
            }
        }
    }
    public bool GetAction()
    {
        return this.action;
    }

    public void FaceTarget(Entity entity)
    {
        Vector3 direction = entity.transform.position - transform.position;
        HandleFlip(direction);
    }

    public void AttackTarget(Entity entity)
    {
        SetAction(false);
        if (TotalMovement > 0)
        {
            FaceTarget(entity);
        }
        if (AnimatorManager.PlayAnimation(animator, AttackAnimation))
        {
            Target = entity;
            animator.Play(AttackAnimation);
        }
        else
        {
            entity.ChangeHealthByAmount(-this.Attack);
        }
    }

    public bool ProduceCard(GhostEntity ghost, GridNode node)
    {
        GridVisual gVisual=Global.GameGrid.gVisual;
        if (GetAction())
        {
            if (ghost.card.isWalkable(gVisual.HighlightedNode) && Global.GameInstance.CurrentTurn && Global.myPlayerResourceManager.Affordable(ghost.card))
            {
                SetAction(false);
                Global.GameInstance.PlaySelectedCard(gVisual.HighlightedNode.GridIndex.x, gVisual.HighlightedNode.GridIndex.y);
                if (animator.runtimeAnimatorController != null && !string.IsNullOrEmpty(ProductionAnimation))
                {
                    animator.Play(ProductionAnimation);
                }
                //gVisual.DisplayMovement();
                return true;
            }
        }
        return false;
    }

 
    public void MoveOrAttack(GridNode node)
    {
        if (action == false) {
            return;
        }
        Entity enemy = EnemyOnNode(node);
        //List<DistanceFromNodeResult> nodes=Global.GameGrid.GetNodesBFSDistanceFromNode(this, this.node, this.Movement);
        //if (DistanceFromNodeResult.Contains(nodes, node)>=0 && enemy!=null)
        if (Grid.GetDistance(this.node, node) <= Range && enemy != null)
        {
            Global.NetManager.Send(AttackPacket.GetBytes(NetID, enemy.NetID));
            return;
        }
        else
        {
            MoveTo(node);
            return;
        }
    }

    public Entity EnemyOnNode(GridNode node)
    {
        foreach (Entity e in node.Occupants)
        {
            if (e.PlayerID!=PlayerID)
            {
                return e;
            }
        }
        return null;
    }

    /*public void DealDamageToTarget()
    {
        if (Target==null)
        {
            return;
        }
        Target.Health -= Attack;
        Target = null;
    }*/

    public bool IsOwner()
    {
        if (Global.PlayerID == this.PlayerID)
        {
            return true;
        }
        return false;
    }

    public GridNode GetNode()
    {
        return node;
    }
    public void SetNode(GridNode node)
    {
        if (this.node != null)
        {
            this.node.Occupants.Remove(this);
        }
        if (node != null)
        {
            this.sprite_renderer.sortingOrder = node.GetOrder() + GridNode.EntityLayer;
            if (entity_type==EntityType.Building)
            {
                this.sprite_renderer.sortingOrder = node.GetOrder() + GridNode.BuildingLayer;
            }
            if (!node.Occupants.Contains(this))
            {
                node.Occupants.Add(this);
            }
        }
        this.node = node;
    }

    void HandleFlip(Vector3 direction)
    {
        if (
            (direction.x>0 && DefaultDirection==Direction.Left) ||
            (direction.x<0 && DefaultDirection==Direction.Right)
        )
        {
            sprite_renderer.flipX = true;
        }
        else if (direction.x!=0)
        {
            sprite_renderer.flipX = false;
        }
    }

    void Move(float delta)
    {
        if (waypoint != node)
        {
            waypoint = node;
        }
        if (waypoint==null || transform.position == waypoint.position)
        {
            isMoving = false;
            return;
        }
        if (speed<=0)
        {
            this.transform.position = waypoint.position;
        }
        float distance=speed*delta;
        Vector3 direction = (waypoint.position - this.transform.position);
        if (direction.sqrMagnitude>distance*distance)
        {
            this.transform.position = this.transform.position+direction.normalized * distance;
        }
        else
        {
            this.transform.position = waypoint.position;
        }
        isMoving = true;
    }

    public void InitializeEntity(int PlayerID, int NetID)
    {
        this.NetID = NetID;
        this.PlayerID = PlayerID;
        Global.GameInstance.EntityList.Add(NetID, this);
        GameObject StatDisplayGameObject= Instantiate(StatDisplayPrefab, Global.myUIEntityDisplayManager.transform);
        this.StatDisplay = StatDisplayGameObject.GetComponent<EntityStatDisplay>();
        StatDisplay.SetTarget(this);
        if (IsOwner())
        {
            Global.myPlayerResourceManager.AddUpkeep(this);
        }
    }

    public void HandleAnimation()
    {
        
        if (this.animator.runtimeAnimatorController == null || waypoint==null)
        {
            return;
        }
        Vector3 direction = waypoint.position - transform.position;
        HandleFlip(direction);
        if (this.transform.position != waypoint.position)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    public bool isOwner()
    {
        if (PlayerID ==Global.PlayerID)
        {
            return true;
        }
        return false;
    }
    public void Die()
    {
        if (HQ)
        {
            Global.NetManager.Send(Packet.GetBytes(PacketType.RequestEndGame));
        }
        if (entity_type == EntityType.Resource)
        {
            node.GetTerrainEntity().Show();
        }
        if (Global.GameGrid.gVisual.SelectedNode == node)
        {
            Global.GameGrid.gVisual.DeselectNode();
        }
        SetNode(null);
        Global.GameInstance.EntityList.Remove(NetID);
        if (isOwner())
        {
            Global.myPlayerResourceManager.SubtractUpkeep(this);
        }
        StatDisplay.Die();
        Destroy(this.gameObject);
    }

    void SetVisual()
    {
        bool friendly=true;
        if (IsOwner())
        {
            friendly = false;
        }
        animator.SetBool("Friendly", friendly);
        animator.Play("BaseState");
    }

    private void Start()
    {        
        //SetVisual();
        if (SummoningSickness)
        {
            SetAction(false);
        }
        /*node = Global.GameGrid.GetNodeFromPosition(transform.position);
        SetNode(node);*/
        if (entity_type==EntityType.Resource)
        {
            node.GetTerrainEntity().Hide();
        }
    }

    void HandleActionDisplay()
    {
        if (!this.isOwner())
        {
            return;
        }
        Color c = sprite_renderer.color;
        if (action || !Global.GameInstance.CurrentTurn || CurrentAnimation==AttackAnimation)
        {
            sprite_renderer.color = new Color(c.r, c.g, c.b, ActiveAlpha);
        }
        else
        {
            sprite_renderer.color = new Color(c.r, c.g, c.b, InactiveAlpha);
        }
    }

    void HandleDie()
    {
        if (animator.runtimeAnimatorController != null)
        {
            CurrentAnimation = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }
        if (Health <= 0 && CurrentAnimation != AttackAnimation)
        {
            Die();
        }
    }
    public bool CanMove()
    {
        if (!GetAction())
        {
            return false;
        }
        if (Movement>0)
        {
            return true;
        }
        return false;
    }

    public bool CanAttack()
    {
        if (!GetAction())
        {
            return false;
        }
        if (Attack > 0)
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update () {
        if (node==null)
        {
            node=Global.GameGrid.GetNodeFromPosition(transform.position);
        }
        HandleAnimation();
        HandleActionDisplay();
        HandleDie();
        Move(Time.deltaTime);
	}
}

