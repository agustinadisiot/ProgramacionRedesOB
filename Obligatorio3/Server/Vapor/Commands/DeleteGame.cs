using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System.Threading.Tasks;

namespace Server
{
    public class DeleteGame : TextCommand
    {
        public DeleteGame(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override Command cmd => Command.DELETE_GAME;

        public override async Task ParsedRequestHandler(string[] req)
        {
            int gameId = parseInt(req[0]);

            BusinessLogicGameCUD GameCUD = BusinessLogicGameCUD.GetInstance();
            GameCUD.DeleteGame(gameId);
            string message = "Juego borrado exitosamente";
            await Respond(message);
        }

        private async Task Respond(string message)
        {
            await SendResponseHeader();
            await SendData(message);
        }
    }
}