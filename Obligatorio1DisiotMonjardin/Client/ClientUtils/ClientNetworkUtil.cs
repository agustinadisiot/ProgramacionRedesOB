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

        public static TcpClient GetNewClientTcpEndpoint()
        {
            var clientIpAddress = SettingsMgr.ReadSetting(ClientConfig.ClientIpConfigKey);
            var clientPort = SettingsMgr.ReadSetting(ClientConfig.ClientPortConfigKey);
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse(clientIpAddress), int.Parse(clientPort));
            return new TcpClient(clientIpEndPoint);
        }

        public static IPEndPoint GetServerEndpoint()
        {
            var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);
            var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);
            return new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
        }
    }
}
