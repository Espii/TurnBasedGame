using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {
    public const short GameVersion = 0;
    public static BuildMenu build_menu = null;
    public static CommandMenu command_menu = null;
    public static CameraManager myCameraManager = null;
    public static NetworkCanvas NetCanvas = null;
    public static SoundManager sound_manager = null;
    public static UIMainMenu myUIMainMenu = null;
    public static UIGameManager myUIGameManager=null;
    public static UIEntityDisplayManager myUIEntityDisplayManager =null;
    public static PlayerResourceManager myPlayerResourceManager = null;
    public static Grid GameGrid = null;
    public static Game GameInstance=null;
    public static NetworkManager NetManager = null;
    public static MouseInputManager MouseManager = null;
    public static CardManager Card_Manager = null;
    public static Spellbook spellbook = null;
    public static LoginInput login_panel = null;
    public static int PlayerID = -1;
    public static Camera MainCamera;


    public static Entity GetFriendlyEntityOnNode(GridNode node)
    {
        if (node == null)
        {
            return null;
        }
        foreach (Entity e in node.Occupants)
        {
            if (e.isOwner())
            {
                return e;
            }
        }
        return null;
    }

    public static Entity GetEntityOnNode(GridNode node)
    {
        if (node == null)
        {
            return null;
        }
        foreach (Entity e in node.Occupants)
        {
            return e;
        }
        return null;
    }

    public static Vector2 GetImageSize(RectTransform container, Sprite sprite)
    {
        if (sprite==null)
        {
            return Vector3.zero;
        }
        float ratio = sprite.bounds.size.y / sprite.bounds.size.x;
        float x = container.rect.width;
        float y = container.rect.height;
        Vector2 size;
        if (ratio >= 1)
        {
            size = new Vector2(y / ratio, y);
        }
        else
        {
            size = new Vector2(x, x * ratio);
        }
        return size;
    }
    public static Entity GetEnemyEntityOnNode(GridNode node)
    {
        if (node == null)
        {
            return null;
        }
        foreach (Entity e in node.Occupants)
        {
            if (!e.isOwner())
            {
                return e;
            }
        }
        return null;
    }

    public static Entity GetEnemyEntityOnNode(Entity entity, GridNode node)
    {
        if (node == null)
        {
            return null;
        }
        foreach (Entity e in node.Occupants)
        {
            if (entity.PlayerID!=e.PlayerID)
            {
                return e;
            }
        }
        return null;
    }

    private void Awake()
    {
        MainCamera = Camera.main;
    }
}
