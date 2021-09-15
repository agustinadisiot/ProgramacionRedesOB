using Common.Protocol;

namespace Common.NetworkUtils.Interfaces
{
    public interface INetworkStreamHandler

    {
        void Write(byte[] data);
        void WriteInt(int data);
        void WriteString(string data);
        void WriteCommand(Command data);


        byte[] Read(int length);
        int ReadInt(int length);
        string ReadString(int length);
        Command ReadCommand();
    }
}