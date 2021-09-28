namespace Common.Protocol
{
    public static class Specification
    {
        public const int HEADER_LENGTH = 3;
        public const int CMD_LENGTH = 2;
        public const int DATA_SIZE_LENGTH = 4;
        public const string FIRST_DELIMITER = "|";
        public const string SECOND_DELIMITER = "~";

        public const string RESPONSE_HEADER = "RES";
        public const string REQUEST_HEADER = "REQ";

        public const int FIXED_FILE_SIZE_LENGTH = 8;
        public const int MAX_PACKET_SIZE = 32768; // 32KB


    };
}