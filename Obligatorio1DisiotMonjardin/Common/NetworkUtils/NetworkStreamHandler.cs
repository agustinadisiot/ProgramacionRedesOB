using System;
using System.Net.Sockets;
using System.Text;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

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

        public byte[] Read(int length)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = _networkStream.Read(data, dataReceived, length - dataReceived);
                if (received == 0)
                {
                    throw new SocketException();
                }
                dataReceived += received;
            }

            return data;
        }

        public Command ReadCommand()
        {
            if (debugging) Console.Write("DEBUG Reading command: ");
            byte[] cmd = Read(Specification.CmdLength);
            if (debugging) Console.WriteLine((Command)BitConverter.ToUInt16(cmd));
            return (Command)BitConverter.ToUInt16(cmd);
        }

        public int ReadInt(int length)
        {
            if (debugging) Console.Write($"DEBUG Reading Int length {length}: ");
            byte[] number = Read(length);
            if (debugging) Console.WriteLine(BitConverter.ToInt32(number));
            return BitConverter.ToInt32(number);
        }

        public long ReadFileSize()
        {
            byte[] number = Read(Specification.FixedFileSizeLength);
            return BitConverter.ToInt64(number);
        }

        public string ReadString(int length)
        {

            if (debugging) Console.Write($"DEBUG Reading String length {length}: ");
            byte[] text = Read(length);
            if (debugging) Console.WriteLine(Encoding.UTF8.GetString(text));
            return Encoding.UTF8.GetString(text);
        }

        public void Write(byte[] data)
        {
            _networkStream.Write(data, 0, data.Length);
        }

        public void WriteCommand(Command data)
        {
            if (debugging) Console.Write($"DEBUG Writing command {data}: ");
            ushort command = (ushort)data;
            byte[] cmd = BitConverter.GetBytes(command);
            Write(cmd);
            if (debugging) Console.WriteLine($" WC Done");
        }

        public void WriteInt(int data)
        {
            if (debugging) Console.Write($"DEBUG Writing int {data}: ");
            byte[] number = BitConverter.GetBytes(data);
            Write(number);
            if (debugging) Console.WriteLine($" WI Done");
        }

        public void WriteFileSize(long data)
        {
            byte[] number = BitConverter.GetBytes(data);
            Write(number);
        }

        public void WriteString(string data)
        {
            if (debugging) Console.Write($"DEBUG Writing string {data}: ");
            byte[] encodedData = Encoding.UTF8.GetBytes(data);
            Write(encodedData);
            if (debugging) Console.WriteLine($" WS Done");
        }
    }
}