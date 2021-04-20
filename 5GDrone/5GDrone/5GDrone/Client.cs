using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Networking.Client;

namespace _5GDrone
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

            byte[] data = new byte[client.ReceiveBufferSize];
            double value;

            while (true)
            {
                Transmit("BMP");
                int bytesReadB = stream.Read(data, 0, client.ReceiveBufferSize);
                string responseB = Convert.ToString(Encoding.ASCII.GetString(data, 0, bytesReadB));           //read the transfered byte array and convert to string

                if (responseB.Contains(bmpSensor))                                                           //check was sensor value was sent
                {
                    //Transmit("HOVER")
                    value = Convert.ToDouble(responseB.Replace(bmpSensor, "")) / 100;                        //remove the identification string. During the transfer the decimal is removed therefore divided by 100
                    Console.WriteLine(bmpSensor + ": " + value);
                    checkHeight(value);
                }

                Transmit("ULT");
                int bytesReadU = stream.Read(data, 0, client.ReceiveBufferSize);
                string responseU = Convert.ToString(Encoding.ASCII.GetString(data, 0, bytesReadU));           //read the transfered byte array and convert to string
                if (responseU.Contains(ultraSensor))
                {
                    //Transmit("HOVER")
                    value = Convert.ToDouble(responseU.Replace(ultraSensor, "")) / 100;
                    Console.WriteLine(ultraSensor + ": " + value);
                    checkDistance(value);
                }
                Thread.Sleep(750); //sleep for 0.75seconds
            }

        }
        
        
        ControlWindow ctrlWin = new ControlWindow();


        //Check if the obtained Barometer value is in range, gives out corresponding replies, value is in meters
        public void checkHeight(double value)
        {
            if (value < min)
            {
                //Transmit("HOVER")
                ctrlWin.lbHeight.Content = value + "m \n You are below the min.";
                test();
            }
            else if(value > max)
            {
                //Transmit("HOVER")
                ctrlWin.lbHeight.Content = value + "m \n You are above the max.";
            }
            else
                ctrlWin.lbHeight.Content = value.ToString();
        }

        //just to test if it will call the changeLableHeight method
        public void test()
        {
            ctrlWin.changeLableHeight("test test test test test");

        }

        //Check if the obtained Ultrasonic value is in range, gives out corresponding replies, value is in cm
        public void checkDistance(double value) 
        {
            if (value <= minDistance)
            {
                //Transmit("HOVER")

                ctrlWin.lbDistance.Content = "Watch out too close!! " + value + "cm";
               
            }
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
