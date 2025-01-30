using System;
using System.Collections.Generic;
using System.Linq;
using TM.BL.Models;

namespace TM.BL.Services
{
    public class LoggingService
    {
        private readonly List<LogEntry> _logs = new List<LogEntry>();

        public void Log(string level, string message)
        {
            _logs.Add(new LogEntry
            {
                Id = _logs.Count + 1,
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message
            });
        }

        public List<LogEntry> GetLogs(int count)
        {
            return _logs.OrderByDescending(log => log.Timestamp).Take(count).ToList();
        }

        public int GetLogCount()
        {
            return _logs.Count;
        }
    }

    
}
