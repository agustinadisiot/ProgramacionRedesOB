namespace Common.Protocol
{
    public static class Specification
    {
        // TODO ser consistente con los nombres (seguramente poner todo en mayuscula)
        public const int HeaderLength = 3;
        public const int CmdLength = 2;
        public const int dataSizeLength = 4;
        public const string delimiter = "/";
        public const string secondDelimiter = "#";
        public const int pageSize = 2;
        public const string responseHeader = "RES";
        public const string requestHeader = "REQ";

        /*public const int FixedFileNameLength = 4;
public const int FixedFileSizeLength = 8;
public const int MaxPacketSize = 32768; // 32KB*/
    }
}