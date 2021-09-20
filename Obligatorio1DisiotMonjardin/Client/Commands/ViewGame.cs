using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Linq;

namespace Client
{
    public class ViewGame : TextCommand
    {
        public ViewGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public GameView SendRequest(string gameID)
        {
            SendHeader();
            SendCommand(Command.VIEW_GAME);

            SendData(gameID);
            return ResponseHandler();
        }

        private GameView ResponseHandler()
        {

            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data);
            Game game = new Game()
            {
                Title = parsedData[0],
                Synopsis = parsedData[1],
                ESRBRating = (Common.ESRBRating)int.Parse(parsedData[2]),
                Genre = parsedData[3]
            };

            GameView gameView = new GameView()
            {
                Game = game,
                IsOwned = ToBooleanFromString(parsedData[parsedData.Length - 2]),
                IsPublisher = ToBooleanFromString(parsedData[parsedData.Length - 1])
            };


            return gameView;
        }

        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }
    }
}