using Common;
using Common.Protocol;
using Common.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public static class ServerProgram
    {

        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static void Main(string[] args)
        {
            
            Server server = new Server();

            Console.WriteLine("Server will start accepting connections from the clients");
            server. StartReceivingConnections();

        }

        
    }
}
