using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
     public class ModifyGame : TextCommand
        
     {
        public ModifyGame(INetworkStreamHandler nwsh) : base(nwsh) { }  

        public string SendRequest(int gameId, Game gameToMod)  //refactoring i guess
        {
            SendHeader();
            SendCommand(Command.MODIFY_GAME);

            string data = "";
            data += gameId.ToString();
            data += Specification.delimiter;
            data += gameToMod.Title;
            data += Specification.delimiter;
            data += gameToMod.Synopsis;
            data += Specification.delimiter;
            data += (int)gameToMod.ESRBRating;
            data += Specification.delimiter;
            data += gameToMod.Genre;
            data += Specification.delimiter;

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