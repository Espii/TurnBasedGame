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

namespace GameServer
{
    public partial class ServerForm : Form
    {
        delegate void AppendServerLogDelegate(string text);
        GameServer server = null;
        public void AppendServerLogLine(string text)
        {
            AppendServerLog(text + "\n");
        }
        public void AppendServerLog(string text)
        {
            if (this.ServerLog.InvokeRequired)
            {
                AppendServerLogDelegate d = new AppendServerLogDelegate(AppendServerLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                ServerLog.AppendText(text);
            }
        }

        public ServerForm()
        {
            InitializeComponent();
            server = new GameServer(this);
            
        }
    }
}
