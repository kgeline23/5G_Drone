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
using System.Windows.Shapes;
//using Networking.Client;

namespace _5GDrone
{
    /// <summary>
    /// Interaction logic for HeightRangeWindow.xaml
    /// </summary>
    public partial class HeightRangeWindow : Window
    {
        public Client client;

        public HeightRangeWindow(Client client)
        {
            this.client = client;

            InitializeComponent();
        }

        private void BtnAssign_Click(object sender, RoutedEventArgs e)
        {
            //get user defined values
            Console.WriteLine(Convert.ToDouble(tbMin.Text) +"    :   " + Convert.ToDouble(tbMax.Text));
            client.assignRange(Convert.ToDouble( tbMin.Text), Convert.ToDouble(tbMax.Text));

            ControlWindow main = new ControlWindow(this.client);
            this.client.setWindow(main);
            main.Show();
            this.Close();
        }

    }
}
