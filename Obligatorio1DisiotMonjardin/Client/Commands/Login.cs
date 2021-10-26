using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client
{
    public class Login : TextCommand
    {
        public Login(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGIN;

        public async Task<bool> SendRequest(string username)
        {

            await SendHeader();

            await SendData(username);
            bool response = await ResponseHandler();
            return response;
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