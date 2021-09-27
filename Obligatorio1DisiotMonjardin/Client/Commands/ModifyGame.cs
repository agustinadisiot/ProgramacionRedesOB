using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
     public class ModifyGame : TextCommand
        
     {
        public ModifyGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.MODIFY_GAME;

        public string SendRequest(int gameId, Game gameToMod)  
        {
            SendHeader();

            string data = "";
            data += gameId.ToString();
            data += Specification.FIRST_DELIMITER;
            data += gameToMod.Title;
            data += Specification.FIRST_DELIMITER;
            data += gameToMod.Synopsis;
            data += Specification.FIRST_DELIMITER;
            data += (int)gameToMod.ESRBRating;
            data += Specification.FIRST_DELIMITER;
            data += gameToMod.Genre;
            data += Specification.FIRST_DELIMITER;

            if(gameToMod.CoverFilePath == "")
            {
                data += 0;
                SendData(data);
            }
            else
            {
                data += 1;
                SendData(data);
                fileNetworkStreamHandler.SendFile(gameToMod.CoverFilePath);
            }

            return ResponseHandler();
        }
        private string ResponseHandler()
        {
            string[] data = GetData();
            string message = data[0];
            return message;

        }
    }
}