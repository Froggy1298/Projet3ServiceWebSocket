using Client.Model;
using Client.VueModele;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ClientVueModele();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string test = ((ComboBox)sender).SelectedItem.ToString();
            if (e.AddedItems[0].ToString() == "Client1")
            {
                MainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#de6d6d"));//Method in HEX
            }
            if (e.AddedItems[0].ToString() == "Client2")
            {
                MainGrid.Background = new SolidColorBrush(Color.FromRgb(222, 192, 109));//Method in RGB
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Livre? book = ((ComboBox)sender).SelectedItem as Livre;
            if (book is not null)
                MonLivre.Navigate(book.MonBook);
        }
    }
}
