using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client
{
    public class Logout : TextCommand
    {
        public Logout(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGOUT;

        public async Task<bool> SendRequest()
        {
            await SendHeader();
            await SendData("");
            return await ResponseHandler();
        }

        private async Task<bool> ResponseHandler()
        {
            string[] data = await GetData();
            bool response = ToBooleanFromString(data[0]);
            return response;
        }

        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }
    }
}