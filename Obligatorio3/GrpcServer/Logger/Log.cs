
using Common;
using Common.Interfaces;
using System;

namespace Server
{
    public class Log
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string GameName { get; set; }
        public string Username { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}
