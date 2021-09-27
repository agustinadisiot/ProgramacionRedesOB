using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Linq;

namespace Client
{
    public class ViewGame : TextCommand
    {
        public ViewGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.VIEW_GAME;

        public GameView SendRequest(string gameId)
        {
            SendHeader();

            SendData(gameId);
            return ResponseHandler();
        }

        private GameView ResponseHandler()
        {

            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = networkStreamHandler.ReadInt(Specification.DATA_SIZE_LENGTH);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = ParseByFirstDelimiter(data);
            Game game = new Game()
            {
                Title = parsedData[0],
                Synopsis = parsedData[1],
                ReviewsRating = int.Parse(parsedData[2]),
                ESRBRating = (Common.ESRBRating)(int.Parse(parsedData[3]) - 1),
                Genre = parsedData[4]
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