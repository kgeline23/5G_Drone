using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Client
{
    public interface IClientConnector
    {
        void OnDisconnect();

        //added below
        bool Connect();

        void Disconnect();

        void Transmit(string command);
    }
}
