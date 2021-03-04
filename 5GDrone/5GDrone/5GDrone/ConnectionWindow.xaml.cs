using _5GDrone.Net;
using Networking;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Networking.Client;


namespace _5GDrone
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        //private DroneClient droneClient;
        private Client client;

        private ThicknessAnimation slideAnimation;
        private Storyboard storyBoard;

        private bool isRegistering;

        public ConnectionWindow(Client client)
        {
            InitializeComponent();              

            if (client != null)
                MessageBox.Show(" DroneClient is not null");           

            this.slideAnimation = new ThicknessAnimation();
            this.slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            this.storyBoard = new Storyboard();
            this.storyBoard.Children.Add(this.slideAnimation);
            this.isRegistering = false;
        }

        public ConnectionWindow()
            : this(null) { }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {


            if (!String.IsNullOrEmpty(txb_Ip.Text) || !String.IsNullOrEmpty(txb_Port.Text))                 
            {
                //this.droneClient = new DroneClient(txb_Ip.Text, int.Parse(txb_Port.Text));
                this.client = new Client(txb_Ip.Text, int.Parse(txb_Port.Text));

                if (!this.client.Connect())
                {
                    lbl_Error.Content = "Connection failed!";
                    lbl_Error.Visibility = Visibility.Visible;
                }
                else
                {
                    stk_Connect.Visibility = Visibility.Collapsed;

                    MainWindow main = new MainWindow(this.client);
                    main.Show();
                    this.Close();
                    
                }
            }
            else
            {
                lbl_Error.Content = "Ip and port cannot be empty!";
                lbl_Error.Visibility = Visibility.Visible;
            }
        }

        private void ShowRegister_Click(object sender, RoutedEventArgs e)
        {
            ScrollDown();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ScrollUp();
        }

        private void ScrollUp()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (this.isRegistering)
                {
                    this.slideAnimation.From = new Thickness(0, -600, 0, 0);
                    this.slideAnimation.To = new Thickness(0, 0, 0, 0);

                    //storyBoard.Begin(stk_Content);
                    this.isRegistering = false;
                }
            }));
        }

        private void ScrollDown()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!this.isRegistering)
                {
                    this.slideAnimation.From = new Thickness(0, 0, 0, 0);
                    this.slideAnimation.To = new Thickness(0, -600, 0, 0);

                    //storyBoard.Begin(stk_Content);
                    this.isRegistering = true;
                }
            }));
        }
    }
}
