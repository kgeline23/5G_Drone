using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;


namespace Networking.Client
{
    public class Client
    {
        private bool isReady;
        private bool isConnected;

        private IPAddress host;                                //parsed IP address
        private int port;

        private TcpClient client;                              
        private NetworkStream stream;

        public Client(string ip, int port)
        {
            this.isReady = IPAddress.TryParse(ip, out host);    //check if given ip is in correct format
            this.isConnected = false;

            this.port = port;
        }

        public bool Connect()
        {
            try
            {
                if (this.isReady && !this.isConnected)         //if given IP was able to be parsed and there is no current connection 
                {
                    string listingText = "Client is listening";
                    string testMsg = "Test";

                    this.isConnected = true;
                    this.client = new TcpClient(this.host.ToString(), this.port);
                    this.stream = this.client.GetStream();

                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(listingText);

                    //---send  init text---
                    Console.WriteLine("Sending : " + listingText);
                    this.stream.Write(bytesToSend, 0, bytesToSend.Length);

                    Thread.Sleep(1000);

                    bytesToSend = ASCIIEncoding.ASCII.GetBytes(testMsg);

                    //---send  init text---
                    this.stream.Write(bytesToSend, 0, bytesToSend.Length);

                    return true;
                }
                else if (!this.isReady)
                {
                    Console.WriteLine("Client could not connect due to invalid ip!\n");
                }
                else
                {
                    Console.WriteLine("Client is already connected!\n");
                }
            }
            catch (Exception e) { }
            return false;
        }

        public void Disconnect()
        {
            if (this.isConnected)
            {
                this.isConnected = false;
                this.client.Close();
                this.stream.Close();
                Console.WriteLine($"Client disconnected on {this.host} using port {this.port}\n");
            }
        }

        public void OnDisconnect()
        {
            //MessageBox.Show("Server closed application wil be closed!");
            Console.WriteLine("Server closed application wil be closed!");
            Environment.Exit(0);
        }

        public void Transmit(string command)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(command);

            //---send  init text---
            Console.WriteLine("Sending : " + command);
            this.stream.Write(bytesToSend, 0, bytesToSend.Length);
        }
    }
}
