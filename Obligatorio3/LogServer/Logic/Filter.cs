using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;

namespace LogServer
{
    public class Filter
    {
        public int? GameId { get; set; }
        public int? UserId { get; set; }
        public string GameName { get; set; }
        public string Username { get; set; }
        public DateTime? MinDateTime { get; set; }
        public DateTime? MaxDateTime { get; set; }
        public string Severity { get; set; }
    }
}
