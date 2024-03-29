﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using static Server.Modele.JsonClass;

namespace Server.Modele
{

    class ServerCommand
    {
        byte[] bytes = new Byte[1024];
        Socket handler = null;

        public ServerCommand() { }
        public Socket BindServerSocket(Settings ServerSettings)
        {

            //IPHostEntry ipHostInfo = Dns.GetHostEntry(ServerSettings.IP);
            //IPEndPoint endPoint = new IPEndPoint(ipHostInfo.AddressList[0], ServerSettings.Port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, ServerSettings.Port);


            Socket serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSock.Bind(endPoint);

            return serverSock;
        }
        public void StartServerListening(ref Socket serverSock,int maxConnection)
        {
            handler.Dispose();
            serverSock.Listen(maxConnection);
        }
        public Socket CloseServerConnection(Socket serverSock)
        {
            try
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                handler = null;
                serverSock.Close();
                serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                return serverSock;
                //ServerSock = ServerCommand.CloseServerConnection(serverSock);

            }
            catch (Exception e)
            {
                MessageBox.Show(Convert.ToString(e));
                return serverSock;
            }
        }
        public Socket CloseClientConnection(Socket clientSock)
        {
            try
            {
                clientSock.Shutdown(SocketShutdown.Both);
                clientSock.Close();
                return clientSock;
            }
            catch (Exception e)
            {
                MessageBox.Show(Convert.ToString(e));
                return clientSock;
            }
        }
        public string GetClientMessage(ref Socket serverSock)
        {
            
            while(true)
            {
                if (handler == null)
                    handler = serverSock.Accept();
                
                string data = null;
                // An incoming connection needs to be processed.  
                while (true)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        // Remove <EOF> in data
                        data = data.Remove(data.IndexOf("<EOF>"), 5);
                        return data;
                    }
                }
            }
        }
        public void SendMessageToClient(string message)
        {
            try
            {
                byte[] messageToSend = Encoding.UTF8.GetBytes(message);
                handler.Send(messageToSend);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
