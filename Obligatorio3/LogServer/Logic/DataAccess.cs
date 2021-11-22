using Common;
using System.Collections.Generic;
using System.Linq;

namespace LogServer
{
    public class DataAccess
    {
        private List<LogRecord> InfoLogs { get; set; }
        private List<LogRecord> WarningLogs { get; set; }
        private List<LogRecord> ErrorLogs { get; set; }
        private List<LogRecord> DefaultLogsList { get; set; }

        private static DataAccess instance;
        private static readonly object singletonPadlock = new object();

        public DataAccess()
        {
            InfoLogs = new List<LogRecord>();
            WarningLogs = new List<LogRecord>();
            ErrorLogs = new List<LogRecord>();
            DefaultLogsList = InfoLogs;
        }
        public static DataAccess GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new DataAccess();
            }
            return instance;
        }

        public void SaveLog(LogRecord log)
        {
            List<LogRecord> correspondingList = null;
            switch (log.Severity)
            {
                case LogRecord.InfoSeverity:
                    correspondingList = InfoLogs;
                    break;
                case LogRecord.WarningSeverity:
                    correspondingList = WarningLogs;
                    break;
                case LogRecord.ErrorSeverity:
                    correspondingList = ErrorLogs;
                    break;
            }

            if (correspondingList == null)
                correspondingList = DefaultLogsList;

            lock (correspondingList)
            {
                correspondingList.Add(log);
            }
        }

        public List<LogRecord> GetLogs(Filter filter)
        {
            List<LogRecord> result = new List<LogRecord>();

            result = result.Concat(FilterList(InfoLogs, filter))
                            .Concat(FilterList(WarningLogs, filter))
                            .Concat(FilterList(ErrorLogs, filter)).ToList();

            return result;
        }

        public List<LogRecord> FilterList(List<LogRecord> list, Filter filter)
        {
            lock (list)
            {
                return list.Where(l => filter.GameId == null || l.GameId == filter.GameId)
                            .Where(l => filter.GameName == null || l.GameName.Contains(filter.GameName))
                            .Where(l => filter.UserId == null || l.UserId == filter.UserId)
                            .Where(l => filter.Username == null || l.Username.Contains(filter.Username))
                            .Where(l => filter.MinDateTime == null || l.DateAndTime >= filter.MinDateTime)
                            .Where(l => filter.MaxDateTime == null || l.DateAndTime <= filter.MaxDateTime)
                            .Where(l => filter.Severity == null || l.Severity == filter.Severity) //TODO ver que hacer
                            .ToList();
            }
        }
    }
}
