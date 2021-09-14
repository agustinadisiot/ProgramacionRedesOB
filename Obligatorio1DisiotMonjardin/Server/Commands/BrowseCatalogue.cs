using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class BrowseCatalogue : TextCommand
    {
        public BrowseCatalogue(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = new Steam();
            string title = Steam.FirstGame();
            Respond(title);
            Console.WriteLine("This is the game list");
        }

        private void Respond(string info)
        {
            byte[] header = Encoding.UTF8.GetBytes("RES");
            ushort command = (ushort)Command.BROWSE_CATALOGUE;
            byte[] cmd = BitConverter.GetBytes(command);

            byte[] data = Encoding.UTF8.GetBytes(info);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            _networkStreamHandler.Write(header); // Header
            _networkStreamHandler.Write(cmd); // CMD
            _networkStreamHandler.Write(dataLength); // Largo
            _networkStreamHandler.Write(data); //data

        }
    }
}
