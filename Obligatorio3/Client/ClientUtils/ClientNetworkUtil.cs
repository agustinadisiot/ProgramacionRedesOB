using Common;
using Common.Interfaces;
using Server;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public static class ClientNetworkUtil
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        public static Socket GetNewSocketClient()
        {
            return new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, 
                ProtocolType.Tcp);
        }

        public static IPEndPoint GetServerEndpoint()
        {
            var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);
            var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);
            return new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
        }
    }
}
