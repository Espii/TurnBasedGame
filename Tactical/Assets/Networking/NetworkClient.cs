using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

// State object for receiving data from remote device.  
public class StateObject
{
    // Client socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 256;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new StringBuilder();
    public List<byte> bl = new List<byte>();
}

public class NetworkClient
{
    // The response from the remote device.  
    ManualResetEvent connectDone = new ManualResetEvent(false);
    Socket client = null;
    public delegate void OnResponseCallbackDelegate(byte[] response);
    public delegate void OnConnectDelegate();

    OnResponseCallbackDelegate OnResponse = null;
    OnConnectDelegate OnConnect = null;

    public NetworkClient(OnConnectDelegate OnConnect, OnResponseCallbackDelegate OnResponse)
    {
        this.OnResponse = OnResponse;
        this.OnConnect = OnConnect;
    }

    public bool Connected()
    {
        if (client==null)
        {
            return false;
        }
        return client.Connected;
    }

    public bool isSocketNull()
    {
        if (client==null)
        {
            return true;
        }
        return false;
    }

    public void Disconnect()
    {
        client.Close();
        client = null;
    }

    public void StartClient(string address, int port, bool LocalHost = false)
    {
        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.  
            // The name of the   
            // remote device is "host.contoso.com"
            IPAddress ipAddress = null;
            if (LocalHost)
            {
                ipAddress = IPAddress.Parse(address);
            }
            else
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
                ipAddress = ipHostInfo.AddressList[0];
            }

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            // Create a TCP/IP socket.  
            client = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            client.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne(1000);
        }
        catch (Exception e)
        {
            Handle_Net_Exception(e);
            Debug.Log(e.ToString());
        }
    }

    public void StopClient()
    {
        // Release the socket.  
        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.  
            client.EndConnect(ar);

            Debug.Log("Socket connected to "+
                client.RemoteEndPoint.ToString());
            OnConnect();
            connectDone.Set();

            this.client = client;
            Receive(client);
        }
        catch (Exception e)
        {
            Handle_Net_Exception(e);
            Debug.Log(e.ToString());
        }
    }

    private void Receive(Socket client)
    {
        try
        {
            if (!client.Connected)
            {
                Debug.Log("Not Connected");
                return;
            }
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.  
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Handle_Net_Exception(e);
            Debug.Log(e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                byte[] sliced = new byte[bytesRead];
                Array.Copy(state.buffer, sliced, bytesRead);
                state.bl.AddRange(sliced);
                
                while (state.bl.Count>=2 && state.bl.Count >= BitConverter.ToInt16(state.bl.ToArray(), 0))
                {
                    int length = BitConverter.ToInt16(state.bl.ToArray(), 0);
                    sliced = new byte[length];
                    Array.Copy(state.bl.ToArray(), sliced, length);
                    state.bl.RemoveRange(0, length);
                    OnResponse.Invoke(sliced);
                }
                /*
                if (state.bl.Count<2 ||(state.bl.Count>=2 && state.bl.Count < BitConverter.ToInt16(state.bl.ToArray(), 0)))
                {
                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                */
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
            }
        }
        catch (Exception e)
        {
            Handle_Net_Exception(e);
            Debug.Log(e.ToString());
        }
    }
    public void Send(String data)
    {
        Send(client, data);
    }

    public void Send(byte[] data)
    {
        Send(client, data);
    }
    public void Send(Socket client, String data)
    {
        if (!client.Connected)
        {
            Debug.Log("Not Connected");
            return;
        }
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);
        Send(client, byteData);
        // Begin sending the data to the remote device.  
        //client.BeginSend(byteData, 0, byteData.Length, 0,
        //    new AsyncCallback(SendCallback), client);
    }

    public void Send(Socket client, Byte[] byteData)
    {
        if (!client.Connected)
        {
            Debug.Log("Not Connected");
            return;
        }
        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), client);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);
            Debug.Log("Sent "+ bytesSent+ " bytes to server.");

            // Signal that all bytes have been sent.  
        }
        catch (Exception e)
        {
            Handle_Net_Exception(e);
            Debug.Log(e.ToString());
        }
    }

    public void Handle_Net_Exception(Exception e)
    {
        if (Global.NetManager != null)
        {
            Global.NetManager.AddDisconnectEvent();
        }
    }
}