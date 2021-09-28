using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;

namespace Server
{
    public class DeleteGame : TextCommand
    {
        public DeleteGame(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override Command cmd => Command.DELETE_GAME;

        public override void ParsedRequestHandler(string[] req)
        {
            int gameId = parseInt(req[0]);

            BusinessLogicGameCUD GameCUD = BusinessLogicGameCUD.GetInstance();
            GameCUD.DeleteGame(gameId);
            string message = "Juego borrado exitosamente";
            Respond(message);
        }

        private void Respond(string message)
        {

            networkStreamHandler.WriteString(Specification.RESPONSE_HEADER);
            networkStreamHandler.WriteCommand(Command.DELETE_GAME);
            networkStreamHandler.WriteInt(message.Length);
            networkStreamHandler.WriteString(message);

        }
    }
}