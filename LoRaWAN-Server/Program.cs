using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWANServer
{
    internal class Program
    {
        static void Main()
        {
            ApplicationServer applicationServer = new ApplicationServer();
            applicationServer.Run("127.0.0.1", 8082);
        }
    }
}
