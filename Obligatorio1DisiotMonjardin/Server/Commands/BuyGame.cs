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
            int gameId;
            bool correctId = int.TryParse(req[0], out gameId);
            // TODO ver que pasa si se parsea mal
            Steam SteamInstance = Steam.GetInstance();
            SteamInstance.BuyGame(gameId, networkStreamHandler);
            string message = "Juego comprado correctamente"; // TODO agregar catch para cuando tira error
            Respond(message);
        }

        private void Respond(string message)
        {
            SendResponseHeader();
            SendData(message);

        }

    }
}
