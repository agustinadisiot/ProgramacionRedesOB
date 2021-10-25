using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System;
using System.Threading.Tasks;

namespace Server.Commands
{
    public abstract class CreateGamePage : TextCommand
    {
        public CreateGamePage(INetworkStreamHandler nwsh) : base(nwsh) { }

        protected BusinessLogicGamePage GamePage;
        public override async Task ParsedRequestHandler(string[] req)
        {
            int pageNumber = parseInt(req[0]); ;

            string unParsedfilter = "";
            if (req.Length > 1)
                unParsedfilter = req[1];

            GamePage = BusinessLogicGamePage.GetInstance();
            GamePage gamePage = GetGamePage(pageNumber, unParsedfilter);
            await Respond(gamePage);
        }

        protected abstract GamePage GetGamePage(int pageNumber, string unParsedfilter);

        protected async Task Respond(GamePage gamePage)
        {
            await SendResponseHeader ();
            string data = "";
            for (int i = 0; i < gamePage.GamesTitles.Count; i++)
            {
                data += gamePage.GamesTitles[i];
                data += Specification.SECOND_DELIMITER;
                data += gamePage.GamesIds[i];
                data += Specification.FIRST_DELIMITER;
            }
            data += Convert.ToInt32(gamePage.HasNextPage);
            data += Specification.FIRST_DELIMITER;
            data += Convert.ToInt32(gamePage.HasPreviousPage);

            await SendData (data);

        }
    }
}
