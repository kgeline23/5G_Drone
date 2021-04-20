﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using _5GDrone;

namespace Networking.Client
{
    public class Client : IClientConnector
    {
        private bool isReady;
        private bool isConnected;

        private IPAddress host;                                 //parsed IP address
        private int port;
        private double min, max;

        private TcpClient client;
        private NetworkStream stream;
        string bmpSensor = "BMP", ultraSensor = "ULT1";         //sensor identification string for transfered data
        int minDistance = 30;                                   //min distance from drone to object (for ultasonic sensor)


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
                if (this.isReady && !this.isConnected)                                                             //if given IP was able to be parsed and there is no current connection 
                {
                    this.isConnected = true;
                    this.client = new TcpClient(this.host.ToString(), this.port);
                    this.stream = this.client.GetStream();

                    Thread.Sleep(1000);

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
            //exit() in python
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

            Console.WriteLine("Sending : " + command);
            this.stream.Write(bytesToSend, 0, bytesToSend.Length);
        }


        public void startSensors()
        {

            Transmit("HEIGHT");
            Transmit("DISTANCE");
            /*
            Dispatcher.Invoke(new Action(() => { lb.Content = "Irantha"; }));


            Func<ControlWindow> fnc = delegate () { return Application.Current.Windows.Cast<Window>().FirstOrDefault(wnd => wnd is ControlWindow) as ControlWindow; };
            MainWindow mnwnd = Application.Current.Dispatcher.Invoke(fnc) as MainWindow;
            mnwnd.status_lable.Dispatcher.Invoke(new Action(() => mnwnd.status_lable.Content = "Irantha signed in"));
            */


            byte[] data = new byte[client.ReceiveBufferSize];
            while (true)
            {
                int bytesRead = stream.Read(data, 0, client.ReceiveBufferSize);
                string response = Convert.ToString(Encoding.ASCII.GetString(data, 0, bytesRead));           //read the transfered byte array and convert to string
                double value;           

                if (response.Contains(bmpSensor))                                                           //check was sensor value was sent
                {
                    //Transmit("HOVER")
                    value = Convert.ToDouble(response.Replace(bmpSensor, "")) / 100;                        //remove the identification string. During the transfer the decimal is removed therefore divided by 100
                    Console.WriteLine(bmpSensor + ": " + value);
                    setHeight(value);
                }
                else if (response.Contains(ultraSensor))
                {
                    //Transmit("HOVER")
                    value = Convert.ToDouble(response.Replace(ultraSensor, "")) / 100;
                    Console.WriteLine(ultraSensor + ": " + value);
                    setDistance(value);
                }
            }

        }

        //starts the barometer, by sending a command and listening for a reponse, reading is in meter
        public string setHeight(double value)
        {
            if (value < min)
            {
                //Transmit("HOVER")
                return value + "m \n You are below the min.";
            }
            else if(value > max)
            {
                //Transmit("HOVER")
                return value + "m \n You are above the max.";
            }
            else
                return value.ToString();
        }

            
        //start the ultrasonic sensor, by sending a command and listening for a reponse, reading is in cm
        public string setDistance(double value) 
        {
            if (value <= minDistance)
            {
                //Transmit("HOVER")

                return "Watch out too close!! " + value + "cm";
            }
            else
                return value.ToString();
        }


/*
        //starts the barometer, by sending a command and listening for a reponse, reading is in meter
        public string getHeight(double value)
        {
            string cmd = "HEIGHT";

            Transmit(cmd);

            byte[] height = new byte[client.ReceiveBufferSize];

            int bytesRead = stream.Read(height, 0, client.ReceiveBufferSize);
            double receivedHeight = Convert.ToDouble(Encoding.ASCII.GetString(height, 0, bytesRead)) / 100;           //during the transfer the decimal is removed therefore divided by 100
            if (receivedHeight < min)
            {
                //Transmit("HOVER")
                return receivedHeight + "m \n You are below the min.";
            }
            else if (receivedHeight > max)
            {
                //Transmit("HOVER")
                return receivedHeight + "m \n You are above the max.";
            }
            else
                return receivedHeight.ToString();
        }


                //start the ultrasonic sensor, by sending a command and listening for a reponse, reading is in cm
        public string getDistance(double value) 
        {
            
            string cmd = "DISTANCE";
            Transmit(cmd);
            int minDistance = 30;

            byte[] distance = new byte[client.ReceiveBufferSize];

            int bytesRead = stream.Read(distance, 0, client.ReceiveBufferSize);
            double receivedDistance = Convert.ToDouble(Encoding.ASCII.GetString(distance, 0, bytesRead)) ; 
            if (receivedDistance <= minDistance)
            {
                //Transmit("HOVER")

                return "Watch out too close!! " + receivedDistance + "cm";
            }
            else
                return receivedDistance.ToString();
        }
        */


        //gets the user defined range from the HeightRangeWindow
        public void assignRange(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

    }
}
