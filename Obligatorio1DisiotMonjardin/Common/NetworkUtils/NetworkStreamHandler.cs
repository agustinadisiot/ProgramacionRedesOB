using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.NetworkUtils
{
    public class NetworkStreamHandler : INetworkStreamHandler
    {
        private readonly NetworkStream _networkStream;
        private const bool debugging = true;

        public NetworkStreamHandler(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public async Task<byte[]> Read(int length)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = await _networkStream.ReadAsync(data, dataReceived, length - dataReceived);
                if (received == 0)
                {
                    throw new SocketException();
                }
                dataReceived += received;
            }

            return data;
        }

        public async Task<Command> ReadCommand()
        {
            if (debugging) Console.Write("DEBUG Reading command: ");
            byte[] cmd = await Read(Specification.CMD_LENGTH);
            if (debugging) Console.WriteLine((Command)BitConverter.ToUInt16(cmd));
            return (Command)BitConverter.ToUInt16(cmd);
        }

        public async Task<int> ReadInt(int length)
        {
            if (debugging) Console.Write($"DEBUG Reading Int length {length}: ");
            byte[] number = await Read(length);
            if (debugging) Console.WriteLine(BitConverter.ToInt32(number));
            return BitConverter.ToInt32(number);
        }

        public async Task<long> ReadFileSize()
        {
            byte[] number = await Read(Specification.FIXED_FILE_SIZE_LENGTH);
            return BitConverter.ToInt64(number);
        }

        public async Task<string> ReadString(int length)
        {
            if (debugging) Console.Write($"DEBUG Reading String length {length}: ");
            byte[] text = await Read(length);
            if (debugging) Console.WriteLine(Encoding.UTF8.GetString(text));
            return Encoding.UTF8.GetString(text);
        }

        public async Task Write(byte[] data)
        {
            await _networkStream.WriteAsync(data, 0, data.Length);
        }

        public async Task WriteCommand(Command data)
        {
            if (debugging) Console.Write($"DEBUG Writing command {data}: ");
            ushort command = (ushort)data;
            byte[] cmd = BitConverter.GetBytes(command);
            await Write(cmd);
            if (debugging) Console.WriteLine($" WC Done");
        }

        public async Task WriteInt(int data)
        {
            if (debugging) Console.Write($"DEBUG Writing int {data}: ");
            byte[] number = BitConverter.GetBytes(data);
            await Write(number);
            if (debugging) Console.WriteLine($" WI Done");
        }

        public async Task WriteFileSize(long data)
        {
            byte[] number = BitConverter.GetBytes(data);
            await Write(number);
        }

        public async Task WriteString(string data)
        {
            if (debugging) Console.Write($"DEBUG Writing string {data}: ");
            byte[] encodedData = Encoding.UTF8.GetBytes(data);
            await Write(encodedData);
            if (debugging) Console.WriteLine($" WS Done");
        }
    }
}