using Microsoft.Extensions.Configuration;
using Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Net;
using Server.VueModele;

namespace Client.VueModele
{
    public class ClientVueModele : INotifyPropertyChanged
    {

        byte[] bytes = new Byte[1024];
        private JsonClass.Settings ClientSettings;
        private IPAddress clientAddress;
        private Socket clientSocket;
        byte[] _receiveBuffer = new byte[2024];
        string fullmessage = null;


        public ClientVueModele()
        {
            PropEstConnectee = "Is Not Connected Yet !";
            var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            var settingTemp = config.GetSection("Settings");
            ClientSettings = settingTemp.Get<JsonClass.Settings>();
            clientAddress = IPAddress.Parse(ClientSettings.IP);
            clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            

            PropConnectServer = new CommandeRelais(ConnectServer);
            PropSendMessage = new CommandeRelais(SendMessage);
            PropReceiveMessage = new CommandeRelais(ReceiveMessage);
            PropDisconnect = new CommandeRelais(Disconnect);


            if (ChoixClient is null) ChoixClient = new();
            for (int i = 1; i < 3; i++)
            {
                PropChoixClient.Add("Client"+i);
            }
        }


        #region Prop Biding de boutton
        public ICommand PropConnectServer { get; set; }
        private void ConnectServer(object param)
        {

            clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(clientAddress, ClientSettings.Port);
            PropEstConnectee = "Is Connected !";
        }
        public ICommand PropSendMessage { get; set; }
        private void SendMessage (object param)
        {
            //CLIENTCONNECTER ; CLIENTENVOI ; MESSAGE
            if(PropClientChoisi == "Client1")
                fullmessage = PropClientChoisi + ";" + "Client2" + ";" + PropMessageEnvoye + "<EOF>";
            if (PropClientChoisi == "Client2")
                fullmessage = PropClientChoisi + ";" + "Client1" + ";" + PropMessageEnvoye + "<EOF>";

            byte[] sendBuffer = Encoding.UTF8.GetBytes(fullmessage);
            clientSocket.Send(sendBuffer);
            bytes = new Byte[1024];


            int bytesMessage = clientSocket.Receive(bytes);
            PropMessageRecu = Encoding.UTF8.GetString(bytes, 0, bytesMessage);

        }
        public ICommand PropReceiveMessage { get; set; }
        private void ReceiveMessage(object param)
        {
            try
            {
                clientSocket.Listen(1);
                PropMessageRecu = GetServerMessage(clientSocket);
            }
            catch(SocketException ex)
            {
                MessageBox.Show("Erreur de reception de message\n" + ex.Message);
            }
        }
        public ICommand PropDisconnect { get; set; }
        private void Disconnect(object param)
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                //clientSocket.Dispose(); //TODO
                PropEstConnectee = "Is Not Connected Yet !";
            }
            finally
            {
                clientSocket.Close();
            }
        }
        #endregion

        #region Prop Binding de message
        private string ClientChoisi;
        public string PropClientChoisi
        {
            get { return ClientChoisi; }
            set { ClientChoisi = value; NotifyPropertyChanged(); }
        }
        private List<string> ChoixClient;
        public List<string> PropChoixClient
        {
            get { return ChoixClient; }
            set { ChoixClient = value; NotifyPropertyChanged(); }
        }
        private string EstConnectee;
        public string PropEstConnectee
        {
            get { return EstConnectee; }
            set { EstConnectee = value; NotifyPropertyChanged(); }
        }
        private string MessageEnvoye;
        public string PropMessageEnvoye
        {
            get { return MessageEnvoye; }
            set { MessageEnvoye = value; NotifyPropertyChanged(); }
        }
        private string MessageRecu;
        public string PropMessageRecu
        {
            get { return MessageRecu; }
            set { MessageRecu = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Poutine de VM
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region fonction
        public string GetServerMessage(Socket clientSocket)
        {

            while (true)
            {
                Socket handler = clientSocket.Accept();
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
        #endregion

    }
}
