using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class TextCommand : CommandHandler
    {
        public TextCommand(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override async Task HandleRequest()
        {
            int length = await networkStreamHandler.ReadInt(Specification.DATA_SIZE_LENGTH);

            string unparsedData = await networkStreamHandler.ReadString(length);

            string[] parsedData = Parse(unparsedData);
            try
            {
                await ParsedRequestHandler(parsedData);
                // TODO aca es el try de todo lo que hace el cliente, poner el log severity= "error"
            }
            catch (ServerError e)
            {
                await SendErrorToClient(e.Message);
            }

        }

        private async Task SendErrorToClient(string message)
        {
            await networkStreamHandler.WriteString(Specification.RESPONSE_HEADER);
            await networkStreamHandler.WriteCommand(Command.ERROR);
            await SendData(message);
        }

        private string[] Parse(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.FIRST_DELIMITER);
            return parsedData;
        }

        protected int parseInt(string unparsedInt)
        {
            bool parseSuccessful = int.TryParse(unparsedInt, out int result);
            if (!parseSuccessful)
                throw new ServerError($"Se esperaba un número pero no se recibió {unparsedInt}");
            return result;
        }

        public abstract Task ParsedRequestHandler(string[] req);

    }
}
