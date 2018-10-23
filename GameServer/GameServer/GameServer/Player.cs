using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace GameServer
{
    public class Player
    {
        public Player(int id, Socket socket, LoginPacket lp)
        {
            this.id = id;
            this.socket = socket;
            this.username = lp.user;
        }
        public int id = -1;
        public string username = string.Empty;
        public Game game = null;
        public int PlayerID = -1; //Player number, ID in-game
        public Socket socket;
    }
}
