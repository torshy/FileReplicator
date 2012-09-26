using System;
using System.Collections.Generic;

namespace TRock.FileReplicator.Services
{
    public interface IActivityLogService
    {
        void Log(Guid id, string activity);

        void ClearLog(Guid id);

        IEnumerable<ActivityLogEvent> GetEvents(Guid id);
    }
}