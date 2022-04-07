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

            //MyPage.Navigate("https://www.gutenberg.org/");
            //MyPage.NavigateToString("https://www.pentagone.com/collections/vetements-homme");

            WebRequest request = WebRequest.Create("https://www.gutenberg.org/files/67785/67785-h/67785-h.htm");


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //WebResponse response = request.GetResponse();


            //Stream dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream);
            StreamReader reader = new StreamReader(response.GetResponseStream());

            

            //string responseFromServer = reader.ReadToEnd();
            List<string> lelivre = new List<string>();
            string test =  "";
            
                while (!reader.EndOfStream)
                {
                    var temp = reader.ReadLine().Replace("&mdash", "").Replace("!"," !").Replace("?", " ?");
                    lelivre.Add(temp);
                    test += temp;
                }
            
           

            //faire un if
            //MessageBox.Show(response.StatusDescription);


          
            MyPage.NavigateToString(test);

        }
    }
}
