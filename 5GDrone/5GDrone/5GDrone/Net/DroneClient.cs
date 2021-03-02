using Networking;
using Networking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _5GDrone.Net
{
    public class DroneClient : IClientConnector
    {
        private Client client;

        public DroneClient(string ip, int port)
        {
            this.client = new Client(ip, port);
            //this.messageReceiver = messageReceiver;
        }

        public bool Connect()
        {
            return this.client.Connect();
        }

        public void Disconnect()
        {
            this.client.Disconnect();
        }

        public void OnDisconnect()
        {
            MessageBox.Show("Server closed application wil be closed!");
            Environment.Exit(0);
        }

        public void Transmit(string command)
        {
            client.Transmit(command);
        }
    }
}
