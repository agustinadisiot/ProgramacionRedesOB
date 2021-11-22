using Common;
using Common.Interfaces;
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
            var serverIpAddress = SettingsMgr.ReadSetting(ClientConfig.ServerIpConfigKey);
            var serverPort = SettingsMgr.ReadSetting(ClientConfig.SeverPortConfigKey);
            return new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
        }
    }
}
