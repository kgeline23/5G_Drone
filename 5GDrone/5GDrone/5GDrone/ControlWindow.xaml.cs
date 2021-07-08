using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using LibVLCSharp.Shared;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace _5GDrone
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {

        public Client client;
        private string msgSend;
        private delegate void getContinousCallback();
        //const string video_url = "rtsp://10.1.1.11:8554/stream";
        const string video_url = "rtsp://192.168.178.15:8554/stream";
        readonly LibVLC _libvlc;

        public ControlWindow(Client client)
        {
            InitializeComponent();
            Core.Initialize();


            this.client = client;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            
            //start a background threat to read sensor values
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                getContinousMeasurements();
            }).Start();
            

            //stream vlc

            _libvlc = new LibVLC();
            vlcPlayer.MediaPlayer = new MediaPlayer(_libvlc);
            var m = new Media(_libvlc, video_url, FromType.FromLocation);
            //The next step is to set the corresponding parameters and play the life of rtsp stream. There is a big gap between setting and not setting. You can comment the code to experience
            m.AddOption(":rtsp-tcp");
            m.AddOption(":clock-synchro=0");
            m.AddOption(":live-caching=0");
            m.AddOption(":network-caching=333");
            m.AddOption(":file-caching=0");
            m.AddOption(":grayscale");
            vlcPlayer.MediaPlayer.Play(m);
        }

        public ControlWindow()
        {
            InitializeComponent();
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
            Console.WriteLine("Clicked");
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


        //reference video for background threading https://www.youtube.com/watch?v=mcxERGerWMk
        private void getContinousMeasurements()
        {
            try
            {
                while (true)
                {
                    int time = 500; // 0.5 second
                    //Dispatcher.BeginInvoke(new getContinousCallback(client.getHeight), DispatcherPriority.Render);
                    Dispatcher.BeginInvoke(new getContinousCallback(client.getDistance), DispatcherPriority.Render);

                    Thread.Sleep(time);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void changeLableHeight(string text)
        {
            lbHeight.Content = text;
        }

        public void changeLableDistance(string text)
        {
            lbDistance.Content = text;
        }
        
        //Control buttons
        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "UP";
            client.Transmit(msgSend);
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "DOWN";
            client.Transmit(msgSend);
        }

        private void BtnRight_Turn_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "TURNRIGHT";
            client.Transmit(msgSend);
        }

        private void BtnLeft_Turn_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "TURNLEFT";
            client.Transmit(msgSend);
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "FORWARDS";
            client.Transmit(msgSend);
        }

        private void BtnBackwards_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "bACKWARDS";
            client.Transmit(msgSend);
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "MOVELEFT";
            client.Transmit(msgSend);
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "MOVERIGHT";
            client.Transmit(msgSend);
        }

        private void BtnHover_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "HOVER";
            client.Transmit(msgSend);
            //MessageBox.Show("Clicked");
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            msgSend = "STOP";
            client.Transmit(msgSend);
        }


    }
}
