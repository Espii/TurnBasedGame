using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Game
    {
        GameServer gameserver = null;
        int NextID=0;
        int CurrentPlayerIndex = 0;

        public List<Player> Players = new List<Player>();
        public List<Entity> Entities = new List<Entity>();

        public int GetNextID()
        {
            return NextID++;
        }
        public int PreviewNextID()
        {
            return NextID;
        }

        public void EndGame()
        {
            Send(Packet.GetBytes(PacketType.EndGame));
            gameserver.ConnectedPlayers.RemoveGame(this);
            foreach (Player p in Players)
            {
                p.game = null;
            }
        }

        public Game(GameServer server)
        {
            this.gameserver = server;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
            player.PlayerID = GetPlayerIndex(player);
            player.game = this;
            gameserver.server.Send(player.socket, FoundMatchPacket.GetBytes(player.PlayerID));
        }

        public Player GetCurrentPlayer()
        {
            return Players[CurrentPlayerIndex];
        }

        public int GetPlayerIndex(Player player)
        {
            for (int i=0; i<Players.Count; i++)
            {
                if (Players[i]==player)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Send(byte[] data)
        {
            foreach(Player p in Players)
            {
                gameserver.server.Send(p.socket, data);
            }
        }

        public int GetCurrentPlayerIndex()
        {
            return CurrentPlayerIndex;
        }

        public Player GetNextUser()
        {
            int NextUserIndex=CurrentPlayerIndex+1;
            if (NextUserIndex >= Players.Count)
            {
                NextUserIndex -= Players.Count;
            }
            return Players[NextUserIndex];
        }
        public void NextTurn()
        {
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= Players.Count)
            {
                CurrentPlayerIndex -= Players.Count;
            }
        }
    }
}
