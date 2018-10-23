using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Timers;
using System.Net.NetworkInformation;


namespace GameServer
{
    public class GameServer
    {
        const short ServerVersion = 0;
        const bool Local = false;
        const bool LocalDB = false;
        const bool SinglePlayer = false;
        const bool CheckLogin = false;
        static int NextNoLoginID = 0;
        public PlayerManager ConnectedPlayers;
        DBConnection myDB;
        ServerForm Form = null;
        PacketManager pManager = new PacketManager();
        string server_address = "ec2-54-241-133-55.us-west-1.compute.amazonaws.com";
        public Server server = null;
        System.Timers.Timer ServerTimer = new System.Timers.Timer();
      
        public GameServer(ServerForm Form)
        {
            myDB = new DBConnection(Form);
            if (LocalDB)
            {
                myDB.Connect("127.0.0.1", 3306, "root", null, "gameserver");
            }
            else
            {
                myDB.Connect("mws-db.c7vqyzzjsjxj.us-west-1.rds.amazonaws.com", 3310, "qkrtjdwls91", "mypassword", "gameserver");
            }
            if (myDB.CheckConnection())
            {
                Form.AppendServerLogLine("Connection to Database Successful");
            }
            this.Form = Form;
            server = new Server(this, Form.AppendServerLog, OnAccept, OnReceive);
            if (Local)
                server.StartListening("127.0.0.1", 11000);
            else
                server.StartListening(server_address, 11000);
            ConnectedPlayers = new PlayerManager(this, Form);
            ServerTimer = new System.Timers.Timer(1000);
            ServerTimer.Elapsed += new ElapsedEventHandler(ServerTick);
            ServerTimer.Interval = 1000;
            ServerTimer.Start();
        }

        public void ShowActiveTcpConnections()
        {
            Console.WriteLine("Active TCP Connections");
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation c in connections)
            {
                Form.AppendServerLogLine(
                    c.LocalEndPoint.ToString() +
                    " <==> " +
                    c.RemoteEndPoint.ToString());
            }
        }

        double elapsed = 0;
        void ServerTick(object source, ElapsedEventArgs e)
        {
            System.Timers.Timer timer=(System.Timers.Timer)source;
            double DeltaTime=timer.Interval;
            elapsed += DeltaTime;
            if (elapsed >= 1000)
            {
                ConnectedPlayers.CheckActivePlayers();
                elapsed -= 1000;
            }
            if (SinglePlayer)
            {
                ConnectedPlayers.HandleMatchMaking(1);
            }
            else
            {
                ConnectedPlayers.HandleMatchMaking(2);
            }
        }


        public void OnAccept(Socket handler)
        {

        }

        public void OnReceive(Socket handler, byte [] response)
        {
            Packet ResponsePacket=pManager.ReadPacket(response);
            if (ResponsePacket.type==PacketType.Login)
            {
                int id = -1;
                LoginPacket lp = (LoginPacket)ResponsePacket;
                DataTable dt=myDB.Query("SELECT id FROM `players` WHERE username=\""+lp.user+"\" and password=\""+lp.pass+"\"");
                if (ServerVersion == lp.version)
                {
                    if (dt.Rows.Count > 0 || !CheckLogin)
                    {
                        if (!CheckLogin)
                            id = NextNoLoginID++;
                        else
                            id = (int)dt.Rows[0][0];
                        ConnectedPlayers.AddPlayer(id, handler, lp);
                        Form.AppendServerLogLine("Player: " + lp.user + " Connected");
                    }
                }
                server.Send(handler, LoginResponsePacket.GetBytes(id));
            }
            else if (ResponsePacket.type == PacketType.FindMatch)
            {
                FindMatchPacket fm=(FindMatchPacket)ResponsePacket;
                Player player = ConnectedPlayers.GetPlayer(handler);
                if (player != null)
                {
                    if (fm.match_type == FindMatchType.practice)
                    {
                        Game game = new Game(this);
                        game.AddPlayer(player);
                        server.Send(game.GetCurrentPlayer().socket, Packet.GetBytes(PacketType.StartTurn));
                        Form.AppendServerLogLine("Found match for users");
                        ConnectedPlayers.ActiveGames.Add(game);
                    }
                    else if (!ConnectedPlayers.PlayersLookingForMatchContains(player))
                    {
                        ConnectedPlayers.AddPlayerLookingForMatch(player);
                        Form.AppendServerLogLine(ConnectedPlayers.GetPlayersLookingForMatchCount() + " players looking for match");
                    }
                }
                else
                {
                    throw new Exception("invalid player looking for match");
                }
            }
            else if (ResponsePacket.type == PacketType.CancelFindMatch)
            {
                Player player = ConnectedPlayers.GetPlayer(handler);
                if (player != null)
                {
                    ConnectedPlayers.RemovePlayerLookingForMatch(player);
                    Form.AppendServerLogLine(ConnectedPlayers.GetPlayersLookingForMatchCount() + " players looking for match");

                }
                else
                {
                    throw new Exception("invalid player cancelling match");
                }
            }
            else if (ResponsePacket.type == PacketType.RequestEndGame)
            {
                Player player = ConnectedPlayers.GetPlayer(handler);
                if (player != null)
                {
                    if (player.game!=null)
                    {
                        player.game.EndGame();
                    }
                }
                else
                {
                    throw new Exception("invalid player, sent end game");
                }
            }
            else if (ResponsePacket.type == PacketType.EndTurn)
            {
                Player player = ConnectedPlayers.GetPlayer(handler);
                if (player!=null)
                {
                    if (player.game == null)
                    {
                        throw new Exception("player, not in game, sent end turn");
                    }
                    if (player.game.GetCurrentPlayer() != player)
                    {
                        //Not current player sent end turn
                        return;
                    }
                    Player NextPlayer = player.game.GetNextUser();
                    player.game.NextTurn();
                    server.Send(NextPlayer.socket, Packet.GetBytes(PacketType.StartTurn));
                }
                else
                {
                    throw new Exception("invalid player, sent end turn");
                }
            }
            else if (ResponsePacket.type == PacketType.RequestEntity)
            {
                Player player = ConnectedPlayers.GetPlayer(handler);
                RequestEntityPacket rp = (RequestEntityPacket)ResponsePacket;
                if (player!=null)
                {
                    if (player.game == null)
                    {
                        throw new Exception("player, not in game, sent request entity");
                    }
                    player.game.Send(CreateEntityPacket.GetBytes(player.PlayerID, player.game.GetNextID(), rp.CardID, rp.x, rp.y));
                }
                else
                {
                    throw new Exception("invalid player, sent end turn");
                }
            }
            else if (ResponsePacket.type == PacketType.MoveEntity)
            {
                Player player = ConnectedPlayers.GetPlayer(handler);
                MoveEntityPacket mp = (MoveEntityPacket)ResponsePacket;
                if (player!=null)
                {
                    if (player.game == null)
                    {
                        throw new Exception("player, not in game, sent move entity");
                    }
                    if (player.game.GetCurrentPlayer() != player)
                    {
                        //Not current player sent end turn
                        return;
                    }
                    player.game.Send(MoveEntityPacket.GetBytes(mp.NetID, mp.x, mp.y));
                }
                else
                {
                    throw new Exception("invalid player, sent move entity");
                }
            }
            else if (ResponsePacket.type==PacketType.Attack)
            {
                Player player = ConnectedPlayers.GetPlayer(handler);
                AttackPacket ap = (AttackPacket)ResponsePacket;
                if (player!=null)
                {
                    if (player.game == null)
                    {
                        throw new Exception("player, not in game, sent move entity");
                    }
                    if (player.game.GetCurrentPlayer() != player)
                    {
                        //Not current player sent end turn
                        return;
                    }
                    player.game.Send(AttackPacket.GetBytes(ap.attacker, ap.defender));
                }
                else
                {
                    throw new Exception("invalid player, sent attack packet");
                }
            }
        }
    }
}
