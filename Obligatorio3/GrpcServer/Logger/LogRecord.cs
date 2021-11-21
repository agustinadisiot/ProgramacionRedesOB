
using Common;
using Common.Interfaces;
using System;

namespace Server
{
    public class LogRecord
    {
        public const string ErrorSeverity = "error";
        public const string WarningSeverity = "warn";
        public const string InfoSeverity = "info";

        public string Message { get; set;}
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string GameName { get; set; }
        public string Username { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Severity { get; set; }
    }
}
