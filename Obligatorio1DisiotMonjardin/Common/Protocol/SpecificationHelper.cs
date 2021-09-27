namespace Common.Protocol
{
    public static class SpecificationHelper
    {
        public static long GetParts(long fileSize)
        {
            var parts = fileSize / Specification.MAX_PACKET_SIZE;
            return parts * Specification.MAX_PACKET_SIZE == fileSize ? parts : parts + 1;
        }
    }
}