using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class CreateEntityInfo {
    public CreateEntityInfo(int PlayerID, int NetID, Entity card, int x, int y)
    {
        this.PlayerID = PlayerID;
        this.NetID = NetID;
        this.card = card;
        position.x = x;
        position.y = y;
    }
    public int PlayerID;
    public int NetID;
    public Entity card;
    public Vector2Int position;
}

class AttackInfo
{
    public AttackInfo(Entity attacker, Entity defender)
    {
        this.attacker = attacker;
        this.defender = defender;
    }
    public Entity attacker;
    public Entity defender;
}

class MoveEntityInfo
{
    public MoveEntityInfo(Entity Target, GridNode Move)
    {
        this.Target = Target;
        this.Move = Move;
    }
    public Entity Target;
    public GridNode Move;
}
public delegate bool EventCallback(object obj);
class EventObject
{
    public EventObject(EventCallback cb, object arg)
    {
        this.cb = cb;
        this.arg = arg;
    }

    public bool invoke()
    {
        return cb.Invoke(arg);
    }
    EventCallback cb;
    object arg;
}


class EventLinkedList
{
    LinkedList<EventObject> events = new LinkedList<EventObject>();
    public void Clear()
    {
        events.Clear();
    }
    public void AddEvent(EventCallback cb, object arg)
    {
        events.AddLast(new EventObject(cb, arg));
    }

    public void AddEvent(EventCallback cb)
    {
        events.AddLast(new EventObject(cb, null));
    }

    public void OnUpdate()
    {
        LinkedListNode<EventObject> node=events.First;
        while (node != null)
        {
            LinkedListNode<EventObject> NextNode = node.Next;
            if (node.Value.invoke())
            {
                events.Remove(node);
            }
            node=NextNode;
        }
    }
}
public class NetworkManager : MonoBehaviour {
    const bool RequireLogin = false;
    const bool Local = false;
    private const float KeepAliveTimer = 1000;
    private static bool created = false;
    public bool FindingMatch = false;
    static bool LoggedIn = false;
    NetworkClient Client = null;
    public string local_address = "127.0.0.1";
    public string server_address = "ec2-54-241-133-55.us-west-1.compute.amazonaws.com";
    public int port = 11000;

    EventLinkedList EventLL = new EventLinkedList();
    public string Menu;
    public string Game;

    PacketManager pManager = new PacketManager();

    void OnConnect()
    {
        LoginInput li = Global.login_panel;
        if (li==null)
        {
            Client.Disconnect();
        }
        byte[] bytes = LoginPacket.GetBytes(Global.GameVersion, li.UserInput.text, li.PassInput.text);
        Client.Send(bytes);
        Debug.Log("Connected");
    }

    bool LoginCallback(object arg)
    {
        if (Global.myUIMainMenu == null)
        {
            return false;
        }
        LoginInput li = Global.login_panel;
        LoggedIn = true;
        return true;
    }


    void DisconnectedMenu()
    {
        if (Global.myUIMainMenu==null || Global.login_panel==null)
            return;
        Global.myUIMainMenu.gameObject.SetActive(false);
        if (RequireLogin)
        {
            Global.login_panel.LoginPanel.SetActive(true);
            Global.login_panel.ConnectPanel.SetActive(false);
        }
        else
        {
            Global.login_panel.LoginPanel.SetActive(false);
            Global.login_panel.ConnectPanel.SetActive(true);
        }
        //Global.connected_panel.gameObject.SetActive(true);
    }

    void ConnectedMenu()
    {
        if (Global.myUIMainMenu == null || Global.login_panel == null)
            return;
        Global.login_panel.LoginPanel.SetActive(false);
        Global.login_panel.ConnectPanel.SetActive(false);
        Global.myUIMainMenu.gameObject.SetActive(true);
    }

    bool DisconnectCallback(object arg)
    {
        if (SceneManager.GetActiveScene().name != Menu)
        {
            SceneManager.LoadScene(Menu);
        }
        else
        {
            DisconnectedMenu();
        }
        if (Client.Connected())
        {
            Client.Disconnect();
        }
        return true;
    }

    void EndGameMessageCallback()
    {
        SceneManager.LoadScene(Menu);
    }
    bool EndGameCallback(object arg)
    {
        Global.NetCanvas.CreateMessageBox("Game", "Game Ended through Disconnect/Victory/Defeat", "Return to Menu", EndGameMessageCallback);
        return true;
    }

    public void AddDisconnectEvent()
    {
        EventLL.AddEvent(DisconnectCallback);
    }

    public bool MatchFoundCallback(object obj)
    {
        Debug.Log("Match Found!");
        SceneManager.LoadScene(Game);
        return true;
    }

    public bool StartTurnCallback(object obj)
    {
        if (Global.GameInstance != null)
        {
            Global.GameInstance.StartTurn();
            return true;
        }
        return false;
    }

    bool CreateEntityCallback(object obj)
    {
        CreateEntityInfo ei = (CreateEntityInfo)obj;
        if (Global.GameInstance != null)
        {
            Global.GameInstance.CreateEntity(ei.PlayerID, ei.NetID, ei.card, ei.position);
            return true;
        }
        return false;
    }

    bool MoveEntityCallback(object obj)
    {
        if (Global.GameInstance == null || Global.MouseManager==null)
        {
            return false;
        }
        MoveEntityInfo mi = (MoveEntityInfo)obj;
        mi.Target.SetNode(mi.Move);
        if (mi.Target.isOwner())
        {
            Global.MouseManager.SelectNode(mi.Move);
            Global.command_menu.command.Move();
        }
        return true;
    }

    bool AttackCallback(object obj)
    {
        AttackInfo ai = (AttackInfo)obj;
        if (ai.attacker.isMoving)
        {
            return false;
        }
        if (ai.attacker.aType == Entity.AttackType.Melee && ai.defender.aType == Entity.AttackType.Melee)
        {
            ai.attacker.AttackTarget(ai.defender);
            ai.defender.AttackTarget(ai.attacker);
        }
        else
        {
            ai.attacker.AttackTarget(ai.defender);
        }
        return true;
    }

    void OnResponse(byte[] response)
    {
        Packet packet= pManager.ReadPacket(response);
        if (packet == null)
        {
            Debug.Log("Unknown packet Received");
        }
        else if (packet.type==PacketType.LoginResponse)
        {
            LoginResponsePacket lp = (LoginResponsePacket)packet;
            if (lp.id==-1)
            {
                AddDisconnectEvent();
            }
            else
            {
                EventLL.AddEvent(LoginCallback);
            }
        }
        else if (packet.type==PacketType.FoundMatch)
        {
            EventLL.Clear();
            FoundMatchPacket fp = (FoundMatchPacket)packet;
            Global.PlayerID = fp.PlayerID;
            EventLL.AddEvent(MatchFoundCallback);
            FindingMatch = false;
        }
        else if (packet.type==PacketType.StartTurn)
        {
            EventLL.AddEvent(StartTurnCallback);
        }
        else if (packet.type==PacketType.CreateEntity)
        {
            CreateEntityPacket cp = (CreateEntityPacket)packet;
            CreateEntityInfo ei = new CreateEntityInfo(cp.PlayerID, cp.NetID, Global.Card_Manager.CardDB[cp.CardID], cp.x, cp.y);
            EventLL.AddEvent(CreateEntityCallback, ei);
        }
        else if (packet.type==PacketType.MoveEntity)
        {
            MoveEntityPacket mp = (MoveEntityPacket)packet;
            Entity target = Global.GameInstance.EntityList[mp.NetID];
            GridNode node = Global.GameGrid.grid[mp.x, mp.y];
            MoveEntityInfo mi = new MoveEntityInfo(target, node);
            EventLL.AddEvent(MoveEntityCallback, mi);
        }
        else if (packet.type==PacketType.Attack)
        {
            AttackPacket ap = (AttackPacket)packet;
            Entity attacker = Global.GameInstance.EntityList[ap.attacker];
            Entity defender = Global.GameInstance.EntityList[ap.defender];
            EventLL.AddEvent(AttackCallback, new AttackInfo(attacker, defender));
        }
        else if (packet.type==PacketType.EndGame)
        {
            EventLL.AddEvent(EndGameCallback);
        }
        
    }
    public void Send_FindMatch(FindMatchType type)
    {
        Client.Send(FindMatchPacket.GetBytes(type));
        Global.NetCanvas.CreateMessageBox("Find Match", "Searching for Match", "Cancel", Cancel_FindMatch);
        FindingMatch = true;

    }

    public void Cancel_FindMatch()
    {
        Client.Send(Packet.GetBytes(PacketType.CancelFindMatch));
        FindingMatch = false;
    }

    public void Send_EndTurn()
    {
        Client.Send(Packet.GetBytes(PacketType.EndTurn));
    }

    public void Send(byte [] data)
    {
        Client.Send(data);
    }

    void OnSceneChanged(Scene current, Scene next)
    {
        if (next.name == Menu)
        {
            if (Client.Connected())
            {
                ConnectedMenu();
            }
            else
            {
                DisconnectedMenu();
            }
        }
    }
    
    public void RequestEntity(int ID, int x, int y)
    {
        Send(RequestEntityPacket.GetBytes(0, x, y));
    }

    // Use this for initialization
    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            Client = new NetworkClient(OnConnect, OnResponse);
            Global.NetManager = this;
            SceneManager.activeSceneChanged += OnSceneChanged;
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        /*Client.StartClient(address, port);
        if (!Client.Connected())
        {
            return;
        }
        */
    }

    public void Connect()
    {
        if (Client.Connected())
        {
            Client.Disconnect();
        }
        string address = local_address;
        if (!Local)
        {
            address = server_address;
        }
        Client.StartClient(address, port, Local);
        if (!Client.Connected())
        {
            return;
        }
    }

    void HandleMenuUI()
    {
        if (Global.myUIMainMenu != null && Global.login_panel != null)
        {
            if (!Client.Connected())
            {
                DisconnectedMenu();
                LoggedIn = false;
            }
            else if (LoggedIn)
            {
                ConnectedMenu();
            }

        }
    }
    
    private void Update()
    {
        EventLL.OnUpdate();
        HandleMenuUI();
    }
}
