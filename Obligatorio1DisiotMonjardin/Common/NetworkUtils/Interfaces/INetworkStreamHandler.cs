using Common.Protocol;
using System.Threading.Tasks;

namespace Common.NetworkUtils.Interfaces
{
    public interface INetworkStreamHandler

    {
        Task Write(byte[] data);
        Task WriteInt(int data);
        Task WriteFileSize(long data);
        Task WriteString(string data);
        Task WriteCommand(Command data);


        Task<byte[]> Read(int length);
        Task<int> ReadInt(int length);
        Task<long> ReadFileSize();
        Task<string> ReadString(int length);
        Task<Command> ReadCommand();
    }
}