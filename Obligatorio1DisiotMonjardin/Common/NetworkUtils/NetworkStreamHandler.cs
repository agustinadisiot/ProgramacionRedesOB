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
            byte[] cmd = Read(Specification.CmdLength);
            return (Command)BitConverter.ToUInt16(cmd);
        }

        public int ReadInt(int length)
        {
            byte[] number = Read(length);
            return BitConverter.ToInt32(number);
        }

        public long ReadFileSize()
        {
            byte[] number = Read(Specification.FixedFileSizeLength);
            return BitConverter.ToInt64(number);
        }

        public string ReadString(int length)
        {
            byte[] text = Read(length);
            return Encoding.UTF8.GetString(text);
        }

        public void Write(byte[] data)
        {
            _networkStream.Write(data, 0, data.Length);
        }

        public void WriteCommand(Command data)
        {
            ushort command = (ushort)data;
            byte[] cmd = BitConverter.GetBytes(command);
            Write(cmd);
        }

        public void WriteInt(int data)
        {
            byte[] number = BitConverter.GetBytes(data);
            Write(number);
        }

        public void WriteFileSize(long data)
        {
            byte[] number = BitConverter.GetBytes(data);
            Write(number);
        }

        public void WriteString(string data)
        {
            byte[] header = Encoding.UTF8.GetBytes(data);
            Write(header);
        }
    }
}