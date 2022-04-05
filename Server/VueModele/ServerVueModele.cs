using Microsoft.Extensions.Configuration;
using Server.Modele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Server.VueModele
{
    class ServerVueModele : INotifyPropertyChanged
    {
        public List<string> dictionnaire;
        private JsonClass.SqlConnection SqlSettings;
        private JsonClass.Settings ServerSettings;
        private Socket ServerSocket;
        private ServerCommand CommandeServeur;
        private BdService DataBaseCommand;
        private byte[] bytes;

        public ServerVueModele()
        {
            dictionnaire = new List<string>();
            LoadDictionnaire();

            var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            var settingTemp = config.GetSection("Settings");
            ServerSettings = settingTemp.Get<JsonClass.Settings>();
            settingTemp = config.GetSection("SqlConnection");
            SqlSettings = settingTemp.Get<JsonClass.SqlConnection>();

            PropOpenConnection = new CommandeRelais(OpenConnection);
            PropCloseConnection = new CommandeRelais(CloseConnection);
            PropListen = new CommandeRelais(Listen);
            PropSendMessage = new CommandeRelais(SendMessage);


            DataBaseCommand = new BdService(SqlSettings);
            CommandeServeur = new ServerCommand();
            bytes = new byte[2024];
        }

        #region Propriete Binding
        private string EstConnectee;
        public string PropEstConnectee 
        {
            get { return EstConnectee; }
            set { EstConnectee = value; NotifyPropertyChanged(); }
        }
        private string MessageRecu;
        public string PropMessageRecu 
        {
            get { return MessageRecu; }
            set { MessageRecu = value; NotifyPropertyChanged(); }
        }
        private string MessageEnvoye;
        public string PropMessageEnvoye
        {
            get { return MessageEnvoye; }
            set { MessageEnvoye = value; NotifyPropertyChanged(); }
        }


        #endregion

        #region Bouton de ma vue
        public ICommand PropOpenConnection { get; set; }
        private void OpenConnection(object param)
        {
            ServerSocket = CommandeServeur.BindServerSocket(ServerSettings);
            PropEstConnectee = "Is Connected !";
        }
        public ICommand PropListen { get; set; }
        private void Listen(object param)
        {
            CommandeServeur.StartServerListening(ref ServerSocket, 2);
            PropMessageRecu = CommandeServeur.GetClientMessage(ServerSocket);
            PropMessageEnvoye = FormatClientMessage(PropMessageRecu);

        }
        public ICommand PropSendMessage { get; set; }
        private void SendMessage(object param)
        {
            CommandeServeur.SendMessageToClient(PropMessageEnvoye);
            /*byte[] sendBuffer = Encoding.UTF8.GetBytes(PropMessageEnvoye);
            ServerSocket.Send(sendBuffer);*/
        }
        public ICommand PropCloseConnection { get; set; }
        private void CloseConnection(object param)
        {
            ServerSocket = CommandeServeur.CloseServerConnection(ServerSocket);
            PropEstConnectee = "Not Connected Yet !";
        }
        #endregion

        #region Poutine de VM
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Commande de Dictionnaire

        private bool LoadDictionnaire()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader("dictionnaire.dic"))
                {
                    while (!streamReader.EndOfStream)
                    {
                        dictionnaire.Add(streamReader.ReadLine().Split(",")[0].Replace("\\-", "-"));
                    }
                    streamReader.Close();
                }
                return true;
            }
            catch(IOException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private List<string> ChercherMotInconnu(string messageRecu)
        {
            string[] splitMessage = messageRecu.ToLower().Replace(",", " ").Replace(".", " ").Replace("  ", " ").Split(" ");
            List<string> unknownWords = new List<string>();
            foreach (string word in splitMessage)
            {
                if(!dictionnaire.Contains(word))
                {
                    unknownWords.Add(word);
                }
            }
            return unknownWords;
        }
        private bool AddWordInDictionnaire(List<string> motsInconnus)
        {
            try
            {
                using(StreamWriter streamWriter = File.AppendText("dictionnaire.dic"))
                {
                    foreach(string word in motsInconnus)
                    {
                        dictionnaire.Add(word);
                        streamWriter.WriteLine(word);
                    }
                }
                return true;
            }
            catch(IOException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        #endregion

        #region Fonctions
        private string FormatClientMessage(string messageRecu)
        {

            List<string> unknownWords = ChercherMotInconnu(messageRecu.Split(";")[2]);
            AddWordInDictionnaire(unknownWords);
            List<string> savedMessages = DataBaseCommand.SelectAllUserMessages(messageRecu.Split(";")[0]);
            DataBaseCommand.AddMessageUser(messageRecu.Split(";")[1], messageRecu.Split(";")[2]);
            StringBuilder formatMessage = new StringBuilder();
            formatMessage.Append("Voici le(les) message(s) enregistré(s) à votre nom dans la base de donné:").AppendLine();
            foreach (string mot in savedMessages)
            {
                formatMessage.Append("-").AppendLine(mot); 
            }
            formatMessage.AppendLine().AppendLine("Voici le message recu par le server:").Append("-").AppendLine(messageRecu.Split(";")[2]).AppendLine().AppendLine("Voici les mots inconnus qui ont été ajouter dans le dictionnaire:");
            foreach (string mot in unknownWords)
            {
                formatMessage.Append("-").AppendLine(mot);
            }
            return formatMessage.ToString();
        }
        #endregion

    }
}
