using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Commands
{
    public class PublishGame : TextCommand
    {
        public PublishGame(INetworkStreamHandler nwsh) : base(nwsh) { }
        public override void ParsedRequestHandler(string[] req)
        {
           /* Game newGame = new Game
            {
                Title = req[0]
                // TODO agregar los parametros de Game que faltan
            };
            Steam SteamInstance = Steam.GetInstance();
            SteamInstance.AddGame(newGame);*/
            // TODO hacer la response  
        }
    }
}
