using System;
using System.Collections.Generic;
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
        List<double> listHeight = new List<double>();
        ControlWindow ctrlWin;

        public Client(string ip, int port)
        {
            this.isReady = IPAddress.TryParse(ip, out host);    //check if given ip is in correct format
            this.isConnected = false;
            this.port = port;
        }

        public void setWindow(ControlWindow window)
        {
            this.ctrlWin = window;
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
        /*
                public void startSensors()
                {

                    byte[] data = new byte[client.ReceiveBufferSize];
                    double value;

                    while (true)
                    {
                        Transmit("BMP");
                        int bytesReadB = stream.Read(data, 0, client.ReceiveBufferSize);
                        double receivedHeight = Convert.ToDouble(Encoding.ASCII.GetString(data, 0, bytesReadB)) / 100;           //during the transfer the decimal is removed therefore divided by 100

                        checkHeight(receivedHeight);
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




                //Check if the obtained Barometer value is in range, gives out corresponding replies, value is in meters
                public void checkHeight(double value)
                {
                    if (value < min)
                    {
                        //Transmit("HOVER")
                        ctrlWin.changeLableHeight(value + "m \n You are below the min.");
                    }
                    else if(value > max)
                    {
                        //Transmit("HOVER")
                        ctrlWin.changeLableHeight(value + "m \n You are above the max.");
                    }
                    else
                        ctrlWin.changeLableHeight(value.ToString());
                }


                //Check if the obtained Ultrasonic value is in range, gives out corresponding replies, value is in cm
                public void checkDistance(double value) 
                {
                    if (value <= minDistance)
                    {
                        //Transmit("HOVER")

                        ctrlWin.changeLableDistance( "Watch out too close!! " + value + "cm");

                    }
                }
        */

        //starts the barometer, by sending a command and listening for a reponse, reading is in meter
        public void getHeight()
        {
            //Get reading and save in an array, calculate the average
            //when new value is read remove latest in array and add the new one to it

            Transmit("BMP");

            byte[] byteHeight = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(byteHeight, 0, client.ReceiveBufferSize);
            double receivedHeight = Convert.ToDouble(Encoding.ASCII.GetString(byteHeight, 0, bytesRead)) / 100;           //during the transfer the decimal is removed therefore divided by 100
            double height = 0, temp = 0;
            int lenght = 10;
            if (listHeight.Count < lenght-1 )
            {
                height = receivedHeight;
                listHeight.Add(receivedHeight);
            }
            else if (listHeight.Count == lenght - 1)
            {
                listHeight.RemoveAt(0);
                listHeight.Add(receivedHeight);

                foreach(double i in listHeight)
                {
                    temp += i;
                }
                height = Math.Round(temp / lenght, 2);
            }

            /*
            if (height < min)
            {
                //Transmit("HOVER")
                ctrlWin.changeLableHeight(height + "m \n You are below the min.");
            }
            else if (height > max)
            {
                //Transmit("HOVER")
                ctrlWin.changeLableHeight(height + "m \n You are below the max.");
            }
            else
                ctrlWin.changeLableHeight(height + "m \n ");
            */

            ctrlWin.changeLableHeight(height + "m \n ");

        }

        //start the ultrasonic sensor, by sending a command and listening for a reponse, reading is in cm
        public void getDistance() 
        {
            Transmit("ULT");

            byte[] distance = new byte[client.ReceiveBufferSize];

            int bytesRead = stream.Read(distance, 0, client.ReceiveBufferSize);
            double receivedDistance = Convert.ToDouble(Encoding.UTF8.GetString(distance, 0, bytesRead)) /100 ; 
            if (receivedDistance <= minDistance)
            {
                //Transmit("HOVER")
                ctrlWin.changeLableDistance(receivedDistance + "cm \r\nWatch out too close!!");
            }
            else
                ctrlWin.changeLableDistance( receivedDistance + "cm");

        }

        //gets the user defined range from the HeightRangeWindow
        public void assignRange(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

    }
}
