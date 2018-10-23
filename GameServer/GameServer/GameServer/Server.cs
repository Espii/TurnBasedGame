using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameServer
{
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
        public List<Byte> bl = new List<Byte>();
    }

    public class Server
    {
        public delegate void AppendLogDelegate(string text);
        public delegate void OnReceiveDelegate(Socket handler, byte[] data);
        public delegate void OnAcceptDelegate(Socket client);

        public static int Send_Timeout_Limit = 10000;
        AppendLogDelegate AppendServerLog = null;
        OnReceiveDelegate OnReceive = null;
        OnAcceptDelegate OnAccept = null;
        GameServer gameserver = null;

        public Server(GameServer gameserver, AppendLogDelegate AppendServerLog, OnAcceptDelegate OnAccept, OnReceiveDelegate OnReceive)
        {
            this.gameserver = gameserver;
            this.AppendServerLog = AppendServerLog;
            this.OnAccept = OnAccept;
            this.OnReceive = OnReceive;
        }
        // Thread signal.  
        //public static ManualResetEvent allDone = new ManualResetEvent(false);
        public void StartListening(string address, int port)
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                //while (true)
                {
                    // Set the event to nonsignaled state.  
                    //allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  

                    //Console.WriteLine("Waiting for a connection...");
                    AppendServerLog.Invoke("Waiting for a connection...\n");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    //allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                AppendServerLog.Invoke(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            //allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            OnAccept(handler);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            try
            {
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                gameserver.ConnectedPlayers.RemovePlayer(handler);
            }

            listener.BeginAccept(
                new AsyncCallback(AcceptCallback),
                listener);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = ar.AsyncState as StateObject;
            if (state==null)
            {
                return;
            }
            Socket handler = state.workSocket;
            try
            {
                String content = String.Empty;
                // Read data from the client socket.   
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));
                    byte[] sliced = new byte[bytesRead];
                    Array.Copy(state.buffer, sliced, bytesRead);
                    state.bl.AddRange(sliced);

                    while (state.bl.Count >= 2 && state.bl.Count >= BitConverter.ToInt16(state.bl.ToArray(), 0))
                    {
                        int length = BitConverter.ToInt16(state.bl.ToArray(), 0);
                        sliced = new byte[length];
                        Array.Copy(state.bl.ToArray(), sliced, length);
                        state.bl.RemoveRange(0, length);
                        OnReceive(handler, sliced);
                    }
                    if (state.bl.Count < 2 || (state.bl.Count >= 2 && state.bl.Count < BitConverter.ToInt16(state.bl.ToArray(), 0)))
                    {
                        // Not all data received. Get more.  
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            catch (Exception e)
            {
                gameserver.ConnectedPlayers.RemovePlayer(handler);
                AppendServerLog.Invoke(e.ToString() + "\n");
            }
        }

        public void Send(Socket handler, byte [] byteData)
        {
            try
            {
                handler.SendTimeout = Send_Timeout_Limit;
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }
            catch (Exception e)
            {
                gameserver.ConnectedPlayers.RemovePlayer(handler);
                AppendServerLog.Invoke(e.ToString()+"\n");
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            //handler.BeginSend(byteData, 0, byteData.Length, 0,
            //    new AsyncCallback(SendCallback), handler);
            Send(handler, byteData);
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            if (handler==null)
            {
                return;
            }
            try
            {
                // Retrieve the socket from the state object.  
                

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                AppendServerLog.Invoke("Sent " + bytesSent + " bytes to client.\n");

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                gameserver.ConnectedPlayers.RemovePlayer(handler);
                AppendServerLog.Invoke(e.ToString() + "\n");
            }
        }
    }
}
