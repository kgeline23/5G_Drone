using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _5GDrone
{
    /// <summary>
    /// Interaction logic for ConnectScreen.xaml
    /// </summary>
    public partial class ConnectScreen : Window
    {
        private string portstring;
        private string ip;
        private const string labelE_Empty = "*ONE OR MORE FIELD(S) ARE EMPTY!*";
        private const string labelE_CharRegex = "*THERE CAN NOT BE ANY LETTERS IN THE PORT FIELD ABOVE!*";

        public ConnectScreen()
        {
            InitializeComponent();
        }

        private void connectionBtn_Click(object sender, RoutedEventArgs e)
        {
            ip = ipTxtBox.Text;
            portstring = portTxtBox.Text;

            if (ip == "" || ip == "" && portstring == "" || portstring == "")
            {
                errorLabel.Content = labelE_Empty;
            }
            else
            {
                if (Regex.IsMatch(portstring, @"^[a-zA-z]+$"))
                {
                    errorLabel.Content = labelE_CharRegex;
                }
                else
                {
                    int port = int.Parse(portstring);

                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Connect(ip, port);

                    //MainWindow main = new MainWindow();
                    //App.Current.MainWindow = main;
                    //this.Close();
                    //main.Show();

                }

            }

        }
    }
}
