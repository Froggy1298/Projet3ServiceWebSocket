using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestReadBook
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //MyPage.Navigate("https://www.youtube.com/");
            //MyPage.NavigateToString("https://www.pentagone.com/collections/vetements-homme");

            //WebRequest request = WebRequest.Create("https://www.gutenberg.org/files/18873/18873-h/18873-h.htm");
            //WebRequest request = WebRequest.Create("https://www.gutenberg.org/files/22266/22266-h/22266-h.htm");
            WebRequest request = WebRequest.Create("https://www.gutenberg.org/files/54419/54419-h/54419-h.htm");




           HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //WebResponse response = request.GetResponse();


            Stream dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream);
            StreamReader reader = new StreamReader(response.GetResponseStream());

            
            //MyPage.Source = new Uri(@"file:///C:/Users/suiss/Desktop/ServiceWeb/Server/MereDeLaMarquise.html");
            //string responseFromServer = reader.ReadToEnd();
            List<string> lelivre = new List<string>();
            
            while (!reader.EndOfStream)
            {
                    
                lelivre
                .Add(reader.ReadLine()
                .Replace("—", "")
                .Replace("&#151;", "")
                .Replace("&mdash;", "")
                .Replace("--", "")
                .Replace("!", " !")
                .Replace("?", " ?")
                .Replace(":", " :")
                .Replace("»", " »")
                .Replace("  ", " ")
                .Replace("«", "« "));
            }
            int debutBody = lelivre.IndexOf("<body>");
      
            int startOfBook = lelivre.FindIndex((line) => line.Contains("*** START OF"));
            for (int i = 0; i < lelivre.Count; i++)
            {
                if (lelivre[i].Contains("*** START OF"))
                {
                    startOfBook = i;
                    break;
                }
            }
            

            
            for (int i = debutBody; i < startOfBook; i++)
            {
                lelivre.RemoveAt(debutBody+1);
            }
            
            for (int i = 0; i < lelivre.Count; i++)
            {
               if(lelivre[i].Contains("<img"))
               {
                    lelivre.RemoveAt(i);
               }
            }
            lelivre.RemoveAt(0);
            lelivre.RemoveAt(0);
           
     
            




            //faire un if
            //MessageBox.Show(response.StatusDescription);
            string combinedString = string.Join("", lelivre);

          

            MyPage.NavigateToString(combinedString);

        }
    }
}
