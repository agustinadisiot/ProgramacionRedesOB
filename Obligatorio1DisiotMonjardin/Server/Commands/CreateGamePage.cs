﻿using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public abstract class CreateGamePage : TextCommand
    {
        public CreateGamePage(INetworkStreamHandler nwsh) : base(nwsh) { }

        protected Steam steamInstance;
        public override void ParsedRequestHandler(string[] req)
        {
            int pageNumber = parseInt(req[0]); ;

            string unParsedfilter = "";
            if (req.Length > 1)
                unParsedfilter = req[1];

            steamInstance = Steam.GetInstance();
            GamePage gamePage = GetGamePage(pageNumber, unParsedfilter);
            Respond(gamePage);
            Console.WriteLine("This is the game list");
        }

        protected abstract GamePage GetGamePage(int pageNumber, string unParsedfilter);

        protected void Respond(GamePage gamePage)
        {
            SendResponseHeader();
            string data = "";
            for (int i = 0; i < gamePage.GamesTitles.Count; i++)
            {
                data += gamePage.GamesTitles[i];
                data += Specification.secondDelimiter;
                data += gamePage.GamesIDs[i];
                data += Specification.delimiter;
            }
            data += Convert.ToInt32(gamePage.HasNextPage);
            data += Specification.delimiter;
            data += Convert.ToInt32(gamePage.HasPreviousPage);

            SendData(data);

        }
    }
}
