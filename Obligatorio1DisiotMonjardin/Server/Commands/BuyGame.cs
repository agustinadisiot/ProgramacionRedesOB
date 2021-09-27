using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class BuyGame : TextCommand
    {
        public BuyGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BUY_GAME;

        public override void ParsedRequestHandler(string[] req)
        {
            int gameId = parseInt(req[0]);
            Steam SteamInstance = Steam.GetInstance();
            bool success = SteamInstance.BuyGame(gameId, networkStreamHandler);
            string message;
            if (success)
                message = "Juego comprado correctamente";
            else
                message = "No se pudo comprar el juego";
            Respond(message);
        }

        private void Respond(string message)
        {
            SendResponseHeader();
            SendData(message);

        }

    }
}
