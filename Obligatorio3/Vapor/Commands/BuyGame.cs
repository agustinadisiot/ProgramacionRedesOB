using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class BuyGame : TextCommand
    {
        public BuyGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BUY_GAME;

        public override async Task ParsedRequestHandler(string[] req)
        {
            int gameId = parseInt(req[0]);
            BusinessLogicGameInfo GameInfo = BusinessLogicGameInfo.GetInstance();
            bool success = GameInfo.BuyGame(gameId, networkStreamHandler);
            string message;
            if (success)
                message = "Juego comprado correctamente";
            else
                message = "No se pudo comprar el juego";
            await Respond(message);
        }

        private async Task Respond(string message)
        {
            await SendResponseHeader();
            await SendData(message);

        }

    }
}
