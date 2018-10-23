using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace GameServer
{
    public class PlayerManager
    {
        Dictionary<int, Player> IdDictionary = new Dictionary<int, Player>();
        Dictionary<Socket, Player> SocketDictionary = new Dictionary<Socket, Player>();
        List<Player> PlayersLookingForMatch = new List<Player>();
        public List<Game> ActiveGames = new List<Game>();

        ServerForm Form = null;
        GameServer gameserver;

        public PlayerManager(GameServer gameserver, ServerForm Form)
        {
            this.Form = Form;
            this.gameserver = gameserver;
        }


        public void HandleMatchMaking(int count)
        {
            if (GetPlayersLookingForMatchCount() < count)
            {
                return;
            }
            if (count == 1)
            {
                Player[] playerArray = GetPlayersLookingForMatch(1);
                Game game = new Game(gameserver);
                foreach (Player p in playerArray)
                {
                    game.AddPlayer(p);
                    RemovePlayerLookingForMatch(p);
                }
                //game.Send(CreateEntityPacket.GetBytes(playerArray[0].PlayerID, game.GetNextID(), 0, 2, 4));
                gameserver.server.Send(game.GetCurrentPlayer().socket, Packet.GetBytes(PacketType.StartTurn));
                Form.AppendServerLogLine("Found match for users");
                ActiveGames.Add(game);
            }
            else
            {
                Player[] playerArray = GetPlayersLookingForMatch(2);
                Game game = new Game(gameserver);
                foreach (Player p in playerArray)
                {
                    game.AddPlayer(p);
                    RemovePlayerLookingForMatch(p);
                }
                gameserver.server.Send(game.GetCurrentPlayer().socket, Packet.GetBytes(PacketType.StartTurn));
                //game.Send(CreateEntityPacket.GetBytes(playerArray[0].PlayerID, game.GetNextID(), 0, 2, 4));
                //game.Send(CreateEntityPacket.GetBytes(playerArray[1].PlayerID, game.GetNextID(), 0, 12, 4));
                Form.AppendServerLogLine("Found match for users");
                ActiveGames.Add(game);
            }
        }
        public int GetPlayersLookingForMatchCount()
        {
            return PlayersLookingForMatch.Count;
        }

        public void RemoveGame(Game game)
        {
            ActiveGames.Remove(game);
        }
        public void CheckActivePlayers()
        {
            /*for (int i = PlayersLookingForMatch.Count - 1; i >= 0; i--)
            {
                Player p = PlayersLookingForMatch[i];
                try
                {
                    gameserver.server.Send(p.socket, Packet.GetBytes(PacketType.Test));
                }
                catch (Exception ex)
                {
                    Form.AppendServerLogLine(ex.Message);
                    //RemovePlayer(p);
                }
            }
            for (int i = ActiveGames.Count - 1; i >= 0; i--)
            {
                Game game = ActiveGames[i];
                try
                {
                    game.Send(Packet.GetBytes(PacketType.Test));
                }
                catch (Exception ex)
                {
                    Form.AppendServerLogLine(ex.Message);
                    //RemovePlayer(p);
                }
            }*/
            List<Socket> sockets = new List<Socket>();
            foreach(KeyValuePair<Socket, Player> kvp in SocketDictionary)
            {
                sockets.Add(kvp.Key);
            }
            foreach (Socket s in sockets)
            {
                gameserver.server.Send(s, Packet.GetBytes(PacketType.Test));
            }
        }

        public Player[] GetPlayersLookingForMatch(int count)
        {
            if (count <= PlayersLookingForMatch.Count)
            {
                return PlayersLookingForMatch.GetRange(0, count).ToArray();
            }
            return null;
        }

        public void RemovePlayerLookingForMatch(Player player)
        {
            PlayersLookingForMatch.Remove(player);
        }

        public void AddPlayerLookingForMatch(Player player)
        {
            PlayersLookingForMatch.Add(player);
        }

        public bool PlayersLookingForMatchContains(Player player)
        {
            if (PlayersLookingForMatch.Contains(player))
            {
                return true;
            }
            return false;
        }
        public void AddPlayer(int id, Socket handler, LoginPacket lp)
        {
            Player player=null;
            IdDictionary.TryGetValue(id, out player);
            if (player == null)
            {
                player = new Player(id, handler, lp);
                IdDictionary.Add(id, player);
            }
            else
            {
                if (player.socket != null)
                {
                    SocketDictionary.Remove(player.socket);
                    player.socket.Close();
                }
                player.socket = handler;
            }
            SocketDictionary.Add(handler, player);
        }

        public Player GetPlayer(Socket handler)
        {
            Player player = null;
            SocketDictionary.TryGetValue(handler, out player);
            return player;
        }

        public Player GetPlayer(int id)
        {
            Player player = null;
            IdDictionary.TryGetValue(id, out player);
            return player;
        }

        public void RemovePlayer(Player player)
        {
            SocketDictionary.Remove(player.socket);
            IdDictionary.Remove(player.id);
            if (PlayersLookingForMatch.Remove(player)) {
                Form.AppendServerLogLine(GetPlayersLookingForMatchCount() + " players looking for match");
            }
            HandlePlayerRemoval(player);
        }

        public void RemovePlayer(Socket handler)
        {
            if (handler == null)
            {
                return;
            }
            Player player = null;
            SocketDictionary.TryGetValue(handler, out player);
            if (player!=null)
            {
                RemovePlayer(player);
            }
            if (handler.Connected)
            {
                handler.Close();
            }
        }

        public void HandlePlayerRemoval(Player player)
        {
            player.socket.Close();
            player.socket = null;
            if (player.game != null)
            {
                player.game.EndGame();
                ActiveGames.Remove(player.game);
                player.game = null;
            }
        }

        public void RemovePlayer(int id)
        {
            Player player = null;
            IdDictionary.TryGetValue(id, out player);
            if (player!=null)
            {
                RemovePlayer(player);
            }
        }
    }
}
