using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Commands
{
    public abstract class CreateGamePage : TextCommand
    {
        protected CreateGamePage(INetworkStreamHandler nwsh) : base(nwsh) { }


        protected async Task<GamePage> ReadGamePage(int pageNumber)
        {
            string[] data = await GetData();

            List<string> gamesInfo = data.ToList();
            gamesInfo.RemoveRange((data.Length - 2), 2);
            List<string> gamesTitles = new List<string>();
            List<string> gameIds = new List<string>();


            foreach (string gameInfo in gamesInfo)
            {
                string[] gameTitleAndId = ParseBySecondDelimiter(gameInfo);
                gamesTitles.Add(gameTitleAndId[0]);
                gameIds.Add(gameTitleAndId[1]);
            }

            GamePage gamePage = new GamePage()
            {
                GamesTitles = gamesTitles,
                GamesIds = gameIds.Select(int.Parse).ToList(),
                CurrentPage = pageNumber,
                HasNextPage = ToBooleanFromString(data[data.Length - 2]),
                HasPreviousPage = ToBooleanFromString(data[data.Length - 1])
            };
            return gamePage;
        }
        protected async Task<GamePage> ResponseHandler(int pageNumber)
        {
            GamePage gamePage = await ReadGamePage(pageNumber);
            return gamePage;

        }

        protected bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }


    }
}
