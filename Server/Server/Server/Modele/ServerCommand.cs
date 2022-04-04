using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using static Server.Modele.JsonClass;

namespace Server.Modele
{
    class ServerCommand
    {
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
            serverSock.Listen(maxConnection);
        }
        public Socket CloseServerConnection(Socket serverSock)
        {
            try
            {
                serverSock.Close();
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
        public Socket GetClientSocket(Socket serverSock)
        {
            return serverSock.Accept();
        }
        public bool SendMessageToClient(string message, Socket client)
        {
            try
            {
                byte[] messageToSend = Encoding.UTF8.GetBytes(message);
                client.Send(messageToSend);
                return true;
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }
}
