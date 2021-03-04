using _5GDrone.Net;
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
using Networking.Client;

namespace _5GDrone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public DroneClient droneClient;
        public Client client;
        private string msgSend;

        public MainWindow(Client client)
        {
            InitializeComponent();
            //this.droneClient = droneClient;
            this.client = client;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    msgSend = "FORWARDS";

                    client.Transmit(msgSend);
                    break;

                case Key.S:
                    msgSend = "BACKWARDS";

                    client.Transmit(msgSend);
                    break;

                case Key.A:
                    msgSend = "MOVELEFT";

                    client.Transmit(msgSend);
                    break;

                case Key.D:
                    msgSend = "MOVERIGHT";

                    client.Transmit(msgSend);
                    break;

                case Key.E:
                    msgSend = "TURNRIGHT";

                    client.Transmit(msgSend);
                    break;

                case Key.Q:
                    msgSend = "TURNLEFT";

                    client.Transmit(msgSend);
                    break;

                case Key.X:
                    msgSend = "UP";

                    client.Transmit(msgSend);
                    break;

                case Key.C:
                    msgSend = "DOWN";

                    client.Transmit(msgSend);
                    break;

                case Key.H:
                    msgSend = "HOVER";

                    client.Transmit(msgSend);
                    break;

                case Key.F:
                    msgSend = "STOP";

                    client.Transmit(msgSend);
                    break;

                default:
                    break;
            }
        }

        private void BtnTakeoff_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "TAKEOFF";

            client.Transmit(msgSend);
        }

        private void BtnLand_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "LAND";

            client.Transmit(msgSend);
        }

        private void BtnEmergency_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "EMERGENCY";

            client.Transmit(msgSend);
        }
    }
}
